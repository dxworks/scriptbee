import * as vscode from 'vscode';
import { scriptSyncService } from './scriptSyncService';
import { connectionService } from './connectionService';
import { getProjectSrcPath } from '../utils/workspaceUtils';
import { logger } from '../utils/logger';
import * as path from 'path';

export class AutoPushService {
  private disposable?: vscode.Disposable;

  public start() {
    this.stop();
    this.disposable = vscode.workspace.onDidSaveTextDocument(async (document) => {
      const config = vscode.workspace.getConfiguration('scriptbee');
      if (!config.get<boolean>('enableAutoPush', true)) {
        return;
      }

      if (document.uri.scheme !== 'file') {
        return;
      }

      try {
        const connectionId = connectionService.getActiveConnectionId();
        if (!connectionId) {
          return;
        }

        const connections = await connectionService.getConnections();
        const connection = connections.find((c) => c.id === connectionId);
        if (!connection || !connection.projectId) {
          return;
        }

        const srcPath = path.normalize(getProjectSrcPath(connection.projectId)).toLowerCase();
        const filePath = path.normalize(document.uri.fsPath).toLowerCase();

        if (filePath.startsWith(srcPath)) {
          await scriptSyncService.pushFileByUri(document.uri);
          logger.log(`Auto-pushed ${document.fileName}`);
        }
      } catch (error) {
        logger.error(`Auto-push failed for ${document.fileName}`, error);
      }
    });
  }

  public stop() {
    if (this.disposable) {
      this.disposable.dispose();
      this.disposable = undefined;
    }
  }
}

export const autoPushService = new AutoPushService();
