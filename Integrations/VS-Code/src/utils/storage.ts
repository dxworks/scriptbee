import * as vscode from 'vscode';

export interface Connection {
  id: string;
  name: string;
  url: string;
  projectId?: string;
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
}

export const storage = Storage.getInstance();
