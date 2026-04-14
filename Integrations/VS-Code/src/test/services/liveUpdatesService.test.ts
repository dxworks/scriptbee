import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import * as signalR from '@microsoft/signalr';
import { LiveUpdatesService, liveUpdatesService, ScriptLiveUpdateEvent } from '../../services/liveUpdatesService';
import { connectionService } from '../../services/connectionService';
import { ClientIdService } from '../../services/clientIdService';
import { scriptSyncService } from '../../services/scriptSyncService';

suite('LiveUpdatesService Tests', () => {
  let getActiveConnectionStub: sinon.SinonStub;
  let pullStub: sinon.SinonStub;
  let mockHubConnection: {
    on: sinon.SinonStub<[string, (event: ScriptLiveUpdateEvent) => Promise<void>], void>;
    start: sinon.SinonStub<unknown[], Promise<void>>;
    stop: sinon.SinonStub<unknown[], Promise<void>>;
    invoke: sinon.SinonStub<unknown[], Promise<unknown>>;
    state: signalR.HubConnectionState;
  };

  setup(async () => {
    await liveUpdatesService.stop();

    mockHubConnection = {
      on: sinon.stub(),
      start: sinon.stub().resolves(),
      stop: sinon.stub().resolves(),
      invoke: sinon.stub().resolves(),
      state: signalR.HubConnectionState.Disconnected,
    };

    sinon.stub(signalR.HubConnectionBuilder.prototype, 'withUrl').returnsThis();
    sinon.stub(signalR.HubConnectionBuilder.prototype, 'withAutomaticReconnect').returnsThis();
    sinon.stub(signalR.HubConnectionBuilder.prototype, 'build').returns(mockHubConnection as unknown as signalR.HubConnection);

    getActiveConnectionStub = sinon.stub(connectionService, 'getActiveConnection');
    pullStub = sinon.stub(scriptSyncService, 'pullFileByUri').resolves();

    sinon.stub(vscode.commands, 'executeCommand').resolves();
    sinon.stub(vscode.workspace, 'findFiles').resolves([]);

    const mockConfig = {
      get: sinon.stub<[string, boolean], boolean>().returns(true),
    } as unknown as vscode.WorkspaceConfiguration;
    sinon.stub(vscode.workspace, 'getConfiguration').returns(mockConfig);

    sinon.stub(LiveUpdatesService.prototype as unknown as { populateCache: () => Promise<void> }, 'populateCache').resolves();
  });

  teardown(async () => {
    await liveUpdatesService.stop();
    sinon.restore();
  });

  test('start should connect to hub if connection is active', async () => {
    getActiveConnectionStub.resolves({ id: 'c1', name: 'T', url: 'http://loc', projectId: 'p1' });

    await liveUpdatesService.start();

    assert.strictEqual(mockHubConnection.start.calledOnce, true);
  });

  test('start should not connect if no active connection', async () => {
    getActiveConnectionStub.resolves(undefined);

    await liveUpdatesService.start();

    assert.strictEqual(mockHubConnection.start.notCalled, true);
  });

  test('should handle ScriptUpdated event', async () => {
    getActiveConnectionStub.resolves({ id: 'c1', name: 'T', url: 'http://loc', projectId: 'p1' });

    await liveUpdatesService.start();

    const call = mockHubConnection.on.getCalls().find((c) => c.args[0] === 'ScriptUpdated');
    assert.ok(call);

    const callback = call.args[1];
    const uri = vscode.Uri.file('/p/s.js');
    liveUpdatesService.updateCacheEntry('s1', uri);

    await callback({ scriptId: 's1', clientId: 'other', projectId: 'p1' });

    assert.strictEqual(pullStub.calledWith(uri), true);
  });

  test('should ignore self events', async () => {
    getActiveConnectionStub.resolves({ id: 'c1', name: 'T', url: 'http://loc', projectId: 'p1' });

    await liveUpdatesService.start();

    const call = mockHubConnection.on.getCalls().find((c) => c.args[0] === 'ScriptUpdated');
    assert.ok(call);

    const callback = call.args[1];

    await callback({
      scriptId: 's1',
      clientId: ClientIdService.clientId,
      projectId: 'p1',
    });

    assert.strictEqual(pullStub.notCalled, true);
  });
});
