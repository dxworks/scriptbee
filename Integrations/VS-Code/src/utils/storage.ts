import * as vscode from 'vscode';

export interface Connection {
  id: string;
  name: string;
  url: string;
  projectId?: string;
  instanceId?: string;
}

export interface ScriptMeta {
  id: string;
  type: 'file' | 'folder';
}

export interface ScriptMeta {
  id: string;
  type: 'file' | 'folder';
}

export class Storage {
  private static instance: Storage;
  private context: vscode.ExtensionContext | undefined;

  private constructor() {}

  public static getInstance(): Storage {
    if (!Storage.instance) {
      Storage.instance = new Storage();
    }
    return Storage.instance;
  }

  public setContext(context: vscode.ExtensionContext) {
    this.context = context;
  }

  public async reset(): Promise<void> {
    if (this.context) {
      this.context.globalState.update('scriptbee.connections', undefined);
      this.context.globalState.update('scriptbee.activeConnectionId', undefined);
    }
  }

  public async getConnections(): Promise<Connection[]> {
    return this.context?.globalState.get<Connection[]>('scriptbee.connections') || [];
  }

  public async saveConnections(connections: Connection[]): Promise<void> {
    await this.context?.globalState.update('scriptbee.connections', connections);
  }

  public getActiveConnectionId(): string | undefined {
    return this.context?.globalState.get<string>('scriptbee.activeConnectionId');
  }

  public async setActiveConnectionId(connectionId: string | undefined): Promise<void> {
    await this.context?.globalState.update('scriptbee.activeConnectionId', connectionId);
  }

  public async getScriptMeta(fileUri: vscode.Uri): Promise<ScriptMeta | undefined> {
    const metaUri = this.getMetaUri(fileUri);
    if (!metaUri) {
      return undefined;
    }

    try {
      const content = await vscode.workspace.fs.readFile(metaUri);
      return JSON.parse(new TextDecoder().decode(content));
    } catch {
      return undefined;
    }
  }

  public async saveScriptMeta(fileUri: vscode.Uri, meta: ScriptMeta): Promise<void> {
    const metaUri = this.getMetaUri(fileUri);
    if (!metaUri) {
      return;
    }

    const parentUri = vscode.Uri.joinPath(metaUri, '..');
    await vscode.workspace.fs.createDirectory(parentUri);

    const content = new TextEncoder().encode(JSON.stringify(meta));
    await vscode.workspace.fs.writeFile(metaUri, content);
  }

  public async deleteScriptMeta(fileUri: vscode.Uri): Promise<void> {
    const metaUri = this.getMetaUri(fileUri);
    if (!metaUri) {
      return;
    }

    try {
      await vscode.workspace.fs.delete(metaUri);
    } catch {}
  }

  private getMetaUri(fileUri: vscode.Uri): vscode.Uri {
    return fileUri.with({ path: `${fileUri.path}.sb.meta` });
  }
}

export const storage = Storage.getInstance();
