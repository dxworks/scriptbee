import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import { autoPushService } from '../../services/autoPushService';
import { connectionService } from '../../services/connectionService';
import { scriptSyncService } from '../../services/scriptSyncService';
import * as workspaceUtils from '../../utils/workspaceUtils';

suite('AutoPushService Tests', () => {
  let onDidSaveStub: sinon.SinonStub;
  let getConfigurationStub: sinon.SinonStub;
  let getActiveConnectionIdStub: sinon.SinonStub;
  let getConnectionsStub: sinon.SinonStub;
  let pushFileByUriStub: sinon.SinonStub;
  let getProjectSrcPathStub: sinon.SinonStub;
  let mockConfig: { get: sinon.SinonStub };
  let saveCallback: (doc: vscode.TextDocument) => Promise<void>;

  setup(() => {
    mockConfig = {
      get: sinon.stub(),
    };
    getConfigurationStub = sinon.stub(vscode.workspace, 'getConfiguration').returns(mockConfig as unknown as vscode.WorkspaceConfiguration);

    onDidSaveStub = sinon.stub(vscode.workspace, 'onDidSaveTextDocument').callsFake((cb) => {
      saveCallback = cb as (doc: vscode.TextDocument) => Promise<void>;
      return { dispose: sinon.stub() } as vscode.Disposable;
    });

    getActiveConnectionIdStub = sinon.stub(connectionService, 'getActiveConnectionId');
    getConnectionsStub = sinon.stub(connectionService, 'getConnections');
    pushFileByUriStub = sinon.stub(scriptSyncService, 'pushFileByUri').resolves();
    getProjectSrcPathStub = sinon.stub(workspaceUtils, 'getProjectSrcPath');

    autoPushService.start();
  });

  teardown(() => {
    autoPushService.stop();
    sinon.restore();
  });

  test('should not push if autoPush is disabled', async () => {
    mockConfig.get.withArgs('enableAutoPush', true).returns(false);

    const doc = {
      uri: vscode.Uri.file('/my/path/src/test.js'),
      fileName: 'test.js',
    } as vscode.TextDocument;

    await saveCallback(doc);

    assert.strictEqual(pushFileByUriStub.notCalled, true);
  });

  test('should not push if URI scheme is not file', async () => {
    mockConfig.get.withArgs('enableAutoPush', true).returns(true);

    const doc = {
      uri: vscode.Uri.parse('untitled:Untitled-1'),
      fileName: 'Untitled-1',
    } as vscode.TextDocument;

    await saveCallback(doc);

    assert.strictEqual(pushFileByUriStub.notCalled, true);
  });

  test('should not push if no active connection', async () => {
    mockConfig.get.withArgs('enableAutoPush', true).returns(true);
    getActiveConnectionIdStub.returns(undefined);

    const doc = {
      uri: vscode.Uri.file('/my/path/src/test.js'),
      fileName: 'test.js',
    } as vscode.TextDocument;

    await saveCallback(doc);

    assert.strictEqual(pushFileByUriStub.notCalled, true);
  });

  test('should not push if file is outside project src directory', async () => {
    mockConfig.get.withArgs('enableAutoPush', true).returns(true);
    getActiveConnectionIdStub.returns('conn1');
    getConnectionsStub.resolves([{ id: 'conn1', projectId: 'proj1' }]);
    getProjectSrcPathStub.withArgs('proj1').returns(vscode.Uri.file('/project/src').fsPath);

    const doc = {
      uri: vscode.Uri.file('/outside/path/test.js'),
      fileName: 'test.js',
    } as vscode.TextDocument;

    await saveCallback(doc);

    assert.strictEqual(pushFileByUriStub.notCalled, true);
  });

  test('should push if file is inside project src directory and autoPush is enabled', async () => {
    mockConfig.get.withArgs('enableAutoPush', true).returns(true);
    getActiveConnectionIdStub.returns('conn1');
    getConnectionsStub.resolves([{ id: 'conn1', projectId: 'proj1' }]);
    getProjectSrcPathStub.withArgs('proj1').returns(vscode.Uri.file('/project/src').fsPath);

    const uri = vscode.Uri.file('/project/src/folder/test.js');
    const doc = {
      uri,
      fileName: 'test.js',
    } as vscode.TextDocument;

    await saveCallback(doc);

    assert.strictEqual(pushFileByUriStub.calledOnceWith(uri), true);
  });
});
