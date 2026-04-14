import * as vscode from 'vscode';
import * as signalR from '@microsoft/signalr';
import { connectionService } from './connectionService';
import { scriptSyncService } from './scriptSyncService';
import { ClientIdService } from './clientIdService';
import { logger } from '../utils/logger';
import * as CommandIds from '../commands/commandIds';

export interface ScriptLiveUpdateEvent {
  projectId: string;
  scriptId: string;
  clientId?: string;
}

export class LiveUpdatesService {
  private hubConnection: signalR.HubConnection | null = null;
  private currentProjectId: string | null = null;
  private currentBaseUrl: string | null = null;
  private scriptIdToUriCache: Map<string, vscode.Uri> = new Map();

  public async start() {
    const connection = await connectionService.getActiveConnection();
    if (!connection || !connection.projectId) {
      await this.stop();
      return;
    }

    const enableLiveUpdates = vscode.workspace.getConfiguration('scriptbee').get<boolean>('enableLiveUpdates', true);
    if (!enableLiveUpdates) {
      await this.stop();
      return;
    }

    if (
      this.currentProjectId === connection.projectId &&
      this.currentBaseUrl === connection.url &&
      this.hubConnection?.state === signalR.HubConnectionState.Connected
    ) {
      return;
    }

    await this.stop();

    this.currentProjectId = connection.projectId;
    this.currentBaseUrl = connection.url;

    await this.populateCache();

    const hubUrl = new URL('/api/projectLiveUpdates', connection.url).toString();

    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(hubUrl).withAutomaticReconnect().build();

    this.hubConnection.on('ScriptCreated', async (event: ScriptLiveUpdateEvent) => {
      if (this.shouldIgnore(event)) {
        return;
      }
      logger.log(`Live Update: Script Created ${event.scriptId}`);
      // TODO: here we can call addNode instead of refreshing the entire tree if we know the parentId
      await vscode.commands.executeCommand(CommandIds.COMMAND_REFRESH_TREE_VIEW);
    });

    this.hubConnection.on('ScriptUpdated', async (event: ScriptLiveUpdateEvent) => {
      if (this.shouldIgnore(event)) {
        return;
      }
      const scriptId = event.scriptId;
      logger.log(`Live Update: Script Updated ${scriptId}`);

      const fileUri = this.scriptIdToUriCache.get(scriptId);
      if (fileUri) {
        await scriptSyncService.pullFileByUri(fileUri);
      }
    });

    this.hubConnection.on('ScriptDeleted', async (event: ScriptLiveUpdateEvent) => {
      if (this.shouldIgnore(event)) {
        return;
      }
      const scriptId = event.scriptId;
      logger.log(`Live Update: Script Deleted ${scriptId}`);
      const fileUri = this.scriptIdToUriCache.get(scriptId);

      if (fileUri) {
        await scriptSyncService.deleteFileByUri(fileUri);
        this.scriptIdToUriCache.delete(scriptId);
      }
      await vscode.commands.executeCommand(CommandIds.COMMAND_REFRESH_TREE_VIEW);
    });

    try {
      await this.hubConnection.start();
      await this.hubConnection.invoke('JoinChannel', this.currentProjectId, 'scripts');
      logger.log(`SignalR connected to ${hubUrl} for project ${this.currentProjectId}`);
    } catch (err) {
      logger.error('Error while starting SignalR connection', err);
    }
  }

  public async stop() {
    if (this.hubConnection) {
      if (this.currentProjectId) {
        try {
          await this.hubConnection.invoke('LeaveChannel', this.currentProjectId, 'scripts');
        } catch (err) {}
      }
      await this.hubConnection.stop();
      this.hubConnection = null;
    }
    this.currentProjectId = null;
    this.currentBaseUrl = null;
    this.scriptIdToUriCache.clear();
  }

  public updateCacheEntry(scriptId: string, uri: vscode.Uri) {
    this.scriptIdToUriCache.set(scriptId, uri);
  }

  private async populateCache() {
    this.scriptIdToUriCache.clear();
    const workspaceFolders = vscode.workspace.workspaceFolders;
    if (!workspaceFolders) {
      return;
    }

    for (const folder of workspaceFolders) {
      const pattern = new vscode.RelativePattern(folder, '**/*.sb.meta');
      const metas = await vscode.workspace.findFiles(pattern, null);

      await Promise.all(
        metas.map(async (metaUri) => {
          try {
            const content = await vscode.workspace.fs.readFile(metaUri);
            const meta = JSON.parse(content.toString());
            if (meta.id) {
              const filePath = metaUri.fsPath.replace('.sb.meta', '');
              this.scriptIdToUriCache.set(meta.id, vscode.Uri.file(filePath));
            }
          } catch (err) {}
        })
      );
    }
  }

  private shouldIgnore(event: ScriptLiveUpdateEvent): boolean {
    return event.clientId === ClientIdService.clientId;
  }
}

export const liveUpdatesService = new LiveUpdatesService();
