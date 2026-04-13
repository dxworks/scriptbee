import { Connection, storage } from '../utils/storage';

export class ConnectionService {
  public async getConnections(): Promise<Connection[]> {
    return await storage.getConnections();
  }

  public getActiveConnectionId(): string | undefined {
    return storage.getActiveConnectionId();
  }

  public async getActiveConnection(): Promise<Connection | undefined> {
    const connections = await this.getConnections();
    const activeId = this.getActiveConnectionId();

    if (!activeId) {
      return undefined;
    }

    return connections.find((c) => c.id === activeId);
  }

  public async addConnection(name: string, url: string): Promise<Connection> {
    const connections = await this.getConnections();
    const newConnection: Connection = {
      id: `${Date.now()}-${Math.floor(Math.random() * 1000)}`,
      name,
      url,
    };
    connections.push(newConnection);
    await storage.saveConnections(connections);

    if (connections.length === 1) {
      await this.setActiveConnection(newConnection.id);
    }

    return newConnection;
  }

  public async updateConnection(connection: Connection): Promise<void> {
    const connections = await this.getConnections();
    const index = connections.findIndex((c) => c.id === connection.id);
    if (index !== -1) {
      connections[index] = connection;
      await storage.saveConnections(connections);
    }
  }

  public async deleteConnection(id: string): Promise<void> {
    let connections = await this.getConnections();
    connections = connections.filter((c) => c.id !== id);
    await storage.saveConnections(connections);

    if (this.getActiveConnectionId() === id) {
      const nextActiveId = connections.length > 0 ? connections[0].id : undefined;
      await this.setActiveConnection(nextActiveId);
    }
  }

  public async setActiveConnection(id: string | undefined): Promise<void> {
    await storage.setActiveConnectionId(id);
  }
}

export const connectionService = new ConnectionService();
