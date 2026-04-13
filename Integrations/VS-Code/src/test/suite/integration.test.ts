import * as assert from 'assert';
import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { projectService } from '../../services/projectService';
import { storage } from '../../utils/storage';

suite('Integration Test Suite', () => {
  setup(async () => {
    if (!(storage as any).context) {
      const mockContext: any = {
        globalState: {
          _data: new Map<string, any>(),
          get: function (key: string) {
            return this._data.get(key);
          },
          update: function (key: string, value: any) {
            this._data.set(key, value);
            return Promise.resolve();
          },
        },
      };
      storage.setContext(mockContext);
    }

    const connections = await connectionService.getConnections();
    for (const conn of connections) {
      await connectionService.deleteConnection(conn.id);
    }
  });

  test('Adding a connection should persist and show in tree', async () => {
    const name = 'Test Connection';
    const url = 'http://localhost:5000';

    await connectionService.addConnection(name, url);

    const connections = await connectionService.getConnections();
    assert.strictEqual(connections.length, 1);
    assert.strictEqual(connections[0].name, name);
    assert.strictEqual(connections[0].url, url);

    const activeConnection = await connectionService.getActiveConnection();
    assert.ok(activeConnection);
    assert.strictEqual(activeConnection?.id, connections[0].id);
  });

  test('Switching connections should update active connection', async () => {
    await connectionService.addConnection('Conn 1', 'http://url1');
    const conn2 = await connectionService.addConnection('Conn 2', 'http://url2');

    await connectionService.setActiveConnection(conn2.id);

    const activeId = connectionService.getActiveConnectionId();
    assert.strictEqual(activeId, conn2.id);
  });

  test('Selecting a project should persist in connection bundle', async () => {
    const conn = await connectionService.addConnection('Conn 1', 'http://url1');
    await connectionService.setActiveConnection(conn.id);

    const projectId = 'project-123';
    await projectService.setSelectedProject(conn.id, projectId);

    const activeConnection = await connectionService.getActiveConnection();
    assert.strictEqual(activeConnection?.projectId, projectId);
  });

  test('Disassociating a project should clear the projectId', async () => {
    const conn = await connectionService.addConnection('Conn 1', 'http://url1');
    await connectionService.setActiveConnection(conn.id);

    await projectService.setSelectedProject(conn.id, 'some-project');
    await projectService.setSelectedProject(conn.id, undefined);

    const updatedConn = await connectionService.getActiveConnection();
    assert.strictEqual(updatedConn?.projectId, undefined);
  });

  test('Setting project for non-active connection should not affect active one', async () => {
    const connA = await connectionService.addConnection('Conn A', 'http://urlA');
    const connB = await connectionService.addConnection('Conn B', 'http://urlB');

    await connectionService.setActiveConnection(connA.id);

    const projectIdB = 'project-B';
    await projectService.setSelectedProject(connB.id, projectIdB);

    const activeConn = await connectionService.getActiveConnection();
    assert.strictEqual(activeConn?.id, connA.id);
    assert.strictEqual(activeConn?.projectId, undefined);

    const connections = await connectionService.getConnections();
    const updatedConnB = connections.find((c) => c.id === connB.id);
    assert.strictEqual(updatedConnB?.projectId, projectIdB);
  });
});
