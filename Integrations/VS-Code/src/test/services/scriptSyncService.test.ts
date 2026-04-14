import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import * as fs from 'fs/promises';
import * as os from 'os';
import * as path from 'path';
import { scriptSyncService } from '../../services/scriptSyncService';
import * as projectFiles from '../../api/projectFiles';
import { storage } from '../../utils/storage';
import { connectionService } from '../../services/connectionService';

suite('ScriptSyncService Tests', function () {
  this.timeout(10000);
  let testRoot: string;
  let getProjectFilesStub: sinon.SinonStub;
  let getScriptContentStub: sinon.SinonStub;
  let deleteProjectFileSpy: sinon.SinonSpy;
  let originalWorkspaceRoot: string | undefined;

  setup(async () => {
    const randomId = Math.random().toString(36).substring(2, 10);
    testRoot = path.join(os.tmpdir(), `scriptbee-test-${randomId}`);
    await fs.mkdir(testRoot, { recursive: true });

    const config = vscode.workspace.getConfiguration('scriptbee');
    originalWorkspaceRoot = config.get('workspaceRoot');
    await config.update('workspaceRoot', testRoot, vscode.ConfigurationTarget.Global);

    getProjectFilesStub = sinon.stub(projectFiles, 'getProjectFiles');
    getScriptContentStub = sinon.stub(projectFiles, 'getScriptContent');
    sinon.stub(projectFiles, 'createScript').resolves({ id: 'new-id', name: 'new', path: 'new', absolutePath: '', language: 'javascript' });
    sinon.stub(projectFiles, 'updateScriptContent').resolves();
    deleteProjectFileSpy = sinon.stub(projectFiles, 'deleteProjectFile').resolves();
  });

  teardown(async () => {
    sinon.restore();
    const config = vscode.workspace.getConfiguration('scriptbee');
    await config.update('workspaceRoot', originalWorkspaceRoot, vscode.ConfigurationTarget.Global);

    try {
      if (testRoot) {
        await fs.rm(testRoot, { recursive: true, force: true });
      }
    } catch (e) {}
  });

  async function createLocalFile(relativePath: string, content: string = 'test') {
    const fullPath = path.join(testRoot, relativePath);
    await fs.mkdir(path.dirname(fullPath), { recursive: true });
    await fs.writeFile(fullPath, content, 'utf8');
    return vscode.Uri.file(fullPath);
  }

  test('pull should delete local orphaned files', async () => {
    const connectionId = 'conn-pull-test';
    const projectId = 'proj-pull-1';

    sinon.stub(connectionService, 'getConnections').resolves([{ id: connectionId, name: 'Test', url: 'http://api-pull', projectId }]);

    const remoteNode = { id: 'remote-id', name: 'remote.js', type: 'file' as const, hasChildren: false, path: 'remote.js', absolutePath: '' };
    getProjectFilesStub.resolves({
      data: [remoteNode],
      totalCount: 1,
      offset: 0,
      limit: 50,
    });
    getScriptContentStub.resolves('remote content');

    const orphanUri = await createLocalFile(path.join('projects', projectId, 'src', 'orphan.js'));
    await storage.saveScriptMeta(orphanUri, { id: 'old-orphaned-id', type: 'file' });

    await scriptSyncService.pull(connectionId);

    const projectSrc = path.join(testRoot, 'projects', projectId, 'src');
    const files = await fs.readdir(projectSrc);

    assert.ok(files.includes('remote.js'), `Files expected to have remote.js, but got: ${files.join(', ')}`);
    assert.ok(!files.includes('orphan.js'), `Orphaned file should have been deleted, but got: ${files.join(', ')}`);
  });

  test('push should NOT delete remote root files when processing an untracked subfolder', async () => {
    const connectionId = 'conn-push-test';
    const projectId = 'proj-push-1';

    sinon.stub(connectionService, 'getConnections').resolves([{ id: connectionId, name: 'Test', url: 'http://api-push', projectId }]);

    const rootNode = { id: 'root-id', name: 'root.js', type: 'file' as const, hasChildren: false, path: 'root.js', absolutePath: '' };
    getProjectFilesStub.resolves({
      data: [rootNode],
      totalCount: 1,
      offset: 0,
      limit: 50,
    });

    await createLocalFile(path.join('projects', projectId, 'src', 'root.js'), 'root content');
    await createLocalFile(path.join('projects', projectId, 'src', 'new-folder', 'file.js'), 'subfolder content');

    await scriptSyncService.push(connectionId);

    assert.strictEqual(deleteProjectFileSpy.callCount, 0, 'No remote deletions should happen during this push');
  });

  test('pushFileByUri should push a single file', async () => {
    const connectionId = 'conn-single-push';
    const projectId = 'proj-single-1';

    sinon.stub(connectionService, 'getActiveConnectionId').returns(connectionId);
    sinon.stub(connectionService, 'getConnections').resolves([{ id: connectionId, name: 'Test', url: 'http://api-single', projectId }]);

    const relativeFilePath = path.join('projects', projectId, 'src', 'script.js');
    const fileUri = await createLocalFile(relativeFilePath, 'new content');
    await storage.saveScriptMeta(fileUri, { id: 'existing-remote-id', type: 'file' });

    await scriptSyncService.pushFileByUri(fileUri);

    const updateStub = projectFiles.updateScriptContent as sinon.SinonStub;
    assert.ok(updateStub.calledOnce, 'updateScriptContent should be called once');
    assert.strictEqual(updateStub.firstCall.args[2], 'existing-remote-id');
    assert.strictEqual(updateStub.firstCall.args[3], 'new content');
  });

  test('pullFileByUri should pull a single file', async () => {
    const connectionId = 'conn-single-pull';
    const projectId = 'proj-single-pull-1';

    sinon.stub(connectionService, 'getActiveConnectionId').returns(connectionId);
    sinon.stub(connectionService, 'getConnections').resolves([{ id: connectionId, name: 'Test', url: 'http://api-pull-single', projectId }]);

    const relativeFilePath = path.join('projects', projectId, 'src', 'pull-script.js');
    const fileUri = await createLocalFile(relativeFilePath, 'old content');
    await storage.saveScriptMeta(fileUri, { id: 'remote-id', type: 'file' });

    const getScriptContentStub = projectFiles.getScriptContent as sinon.SinonStub;
    getScriptContentStub.resolves('remote updated content');

    await scriptSyncService.pullFileByUri(fileUri);

    const newContent = await fs.readFile(fileUri.fsPath, 'utf8');
    assert.strictEqual(newContent, 'remote updated content');
  });
});
