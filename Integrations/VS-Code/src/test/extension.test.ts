import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import { connectionService } from '../services/connectionService';
import * as CommandIds from '../commands/commandIds';
import { scriptSyncService } from '../services/scriptSyncService';
import axiosMockAdapter from 'axios-mock-adapter';
import axiosInstance from '../api/axiosInstance';
import { storage } from '../utils/storage';

suite('ScriptBee Extension UI Command Integration Tests', () => {
  let showInputBoxStub: sinon.SinonStub;
  let showQuickPickStub: sinon.SinonStub;
  let showInformationMessageStub: sinon.SinonStub;
  let axiosMock: axiosMockAdapter;

  setup(() => {
    showInputBoxStub = sinon.stub(vscode.window, 'showInputBox');
    showQuickPickStub = sinon.stub(vscode.window, 'showQuickPick');
    showInformationMessageStub = sinon.stub(vscode.window, 'showInformationMessage');
    axiosMock = new axiosMockAdapter(axiosInstance);
  });

  teardown(() => {
    sinon.restore();
    axiosMock.restore();
  });

  test('Command scriptbee.addConnection should prompt and save connection', async () => {
    showInputBoxStub.onFirstCall().resolves('Test Connection');
    showInputBoxStub.onSecondCall().resolves('http://localhost:5000');

    const addConnectionSpy = sinon.spy(connectionService, 'addConnection');

    await vscode.commands.executeCommand(CommandIds.COMMAND_ADD_CONNECTION);

    assert.ok(addConnectionSpy.calledWith('Test Connection', 'http://localhost:5000'));

    const connections = await connectionService.getConnections();
    const added = connections.find((c) => c.name === 'Test Connection');
    assert.ok(added);
    assert.strictEqual(added.url, 'http://localhost:5000');
  });

  test('Command scriptbee.selectProject should fetch projects and update connection', async () => {
    const testUrl = 'http://test-url';
    const testConnection = await connectionService.addConnection('SelectProjTest', testUrl);
    await connectionService.setActiveConnection(testConnection.id);

    axiosMock.onGet(/\/api\/projects$/).reply((config) => {
      if (config.baseURL === testUrl || config.url?.startsWith(testUrl)) {
        return [200, { data: [{ id: 'proj-a', name: 'Project A' }] }];
      }
      return [404];
    });

    showQuickPickStub.resolves({ label: 'Project A', projectId: 'proj-a' });

    await vscode.commands.executeCommand(CommandIds.COMMAND_SELECT_PROJECT);

    const connections = await connectionService.getConnections();
    const updated = connections.find((c) => c.id === testConnection.id);
    assert.strictEqual(updated?.projectId, 'proj-a');
  });

  test('Command scriptbee.syncScripts should trigger with progress UI', async () => {
    const conn = await setupTestConnection('SyncTest', 'test-id');
    const syncStub = sinon.stub(scriptSyncService, 'sync').resolves();

    await vscode.commands.executeCommand(CommandIds.COMMAND_SYNC_SCRIPTS, { connection: conn });

    assert.ok(syncStub.calledWith(conn.id));
  });

  test('Command scriptbee.pullScripts should trigger with progress UI', async () => {
    const conn = await setupTestConnection('PullTest', 'pull-id');
    const pullStub = sinon.stub(scriptSyncService, 'pull').resolves();

    await vscode.commands.executeCommand(CommandIds.COMMAND_PULL_SCRIPTS, { connection: conn });

    assert.ok(pullStub.calledWith(conn.id));
  });

  test('Command scriptbee.pullScripts should display proper error when server is down', async () => {
    const conn = await setupTestConnection('PullErrorTest', 'pull-err-id');
    const axiosError = new Error('Network Error') as any;
    axiosError.isAxiosError = true;
    axiosError.response = undefined;

    sinon.stub(scriptSyncService, 'pull').rejects(axiosError);
    const showErrorMessageStub = sinon.stub(vscode.window, 'showErrorMessage').resolves(undefined);

    await vscode.commands.executeCommand(CommandIds.COMMAND_PULL_SCRIPTS, { connection: conn });

    assert.ok(
      showErrorMessageStub.calledWithMatch(sinon.match(/Failed to pull scripts: Network Error/)),
      'Should display correct network error instead of undefined'
    );
    showErrorMessageStub.restore();
  });

  test('Command scriptbee.pushScripts should trigger with progress UI', async () => {
    const conn = await setupTestConnection('PushTest', 'push-id');
    const pushStub = sinon.stub(scriptSyncService, 'push').resolves();

    await vscode.commands.executeCommand(CommandIds.COMMAND_PUSH_SCRIPTS, { connection: conn });

    assert.ok(pushStub.calledWith(conn.id));
  });

  test('Command scriptbee.compareWithRemote should trigger vscode.diff', async () => {
    const fileUri = vscode.Uri.file('/fake/project/script.js');
    const meta = { id: 'remote-id-123', type: 'file' as const };

    sinon.stub(storage, 'getScriptMeta').resolves(meta);
    const conn = await setupTestConnection('DiffTest', 'diff-proj-id');

    const executeCommandSpy = sinon.spy(vscode.commands, 'executeCommand');

    await vscode.commands.executeCommand(CommandIds.COMMAND_COMPARE_WITH_REMOTE, fileUri);

    const diffCall = executeCommandSpy.getCalls().find((c) => c.args[0] === 'vscode.diff');
    assert.ok(diffCall);

    const leftUri = diffCall.args[1] as vscode.Uri;
    assert.strictEqual(leftUri.scheme, fileUri.scheme);
    assert.strictEqual(leftUri.fsPath, fileUri.fsPath);

    const rightUri = diffCall.args[2] as vscode.Uri;
    assert.strictEqual(rightUri.scheme, 'scriptbee-remote');
    assert.ok(rightUri.path.includes('remote-id-123'));
    assert.ok(decodeURIComponent(rightUri.query).includes(conn.url));
  });

  async function setupTestConnection(name: string, projectId: string) {
    const conn = await connectionService.addConnection(name, 'http://test-url');
    conn.projectId = projectId;
    await connectionService.updateConnection(conn);
    await connectionService.setActiveConnection(conn.id);
    return conn;
  }
});
