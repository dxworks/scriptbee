import * as assert from 'assert';
import * as sinon from 'sinon';
import * as path from 'path';
import * as fs from 'fs/promises';
import * as os from 'os';
import { scriptSyncService } from '../../../services/scriptSyncService';
import { connectionService } from '../../../services/connectionService';
import * as projectFilesApi from '../../../api/projectFiles';
import * as workspaceUtils from '../../../utils/workspaceUtils';

suite('ScriptSyncService Test Suite', () => {
  let tmpDir: string;
  let getConnectionsStub: sinon.SinonStub;
  let getProjectSrcPathStub: sinon.SinonStub;
  let getProjectFilesStub: sinon.SinonStub;
  let getScriptContentStub: sinon.SinonStub;
  let updateScriptContentStub: sinon.SinonStub;
  let createScriptStub: sinon.SinonStub;

  const baseUrl = 'http://test-url';
  const projectId = 'test-project';
  const connectionId = 'conn-1';

  setup(async () => {
    tmpDir = await fs.mkdtemp(path.join(os.tmpdir(), 'scriptbee-test-'));

    getConnectionsStub = sinon.stub(connectionService, 'getConnections');
    getConnectionsStub.resolves([{ id: connectionId, name: 'Test', url: baseUrl, projectId: projectId }]);

    getProjectSrcPathStub = sinon.stub(workspaceUtils, 'getProjectSrcPath');
    getProjectSrcPathStub.returns(tmpDir);

    getProjectFilesStub = sinon.stub(projectFilesApi, 'getProjectFiles');
    getScriptContentStub = sinon.stub(projectFilesApi, 'getScriptContent');
    updateScriptContentStub = sinon.stub(projectFilesApi, 'updateScriptContent');
    createScriptStub = sinon.stub(projectFilesApi, 'createScript');
  });

  teardown(async () => {
    sinon.restore();
    await fs.rm(tmpDir, { recursive: true, force: true });
  });

  test('pull should handle multi-page pagination', async () => {
    const limit = 2;
    getProjectFilesStub.withArgs(baseUrl, projectId, undefined, 0, 50).resolves({
      data: [
        { id: '1', name: 's1.cs', type: 'file', hasChildren: false },
        { id: '2', name: 's2.cs', type: 'file', hasChildren: false },
      ],
      totalCount: 3,
      offset: 0,
      limit: 2,
    });

    getProjectFilesStub.withArgs(baseUrl, projectId, undefined, 2, 50).resolves({
      data: [{ id: '3', name: 's3.cs', type: 'file', hasChildren: false }],
      totalCount: 3,
      offset: 2,
      limit: 2,
    });

    getScriptContentStub.resolves('content');

    await scriptSyncService.pull(connectionId);

    assert.strictEqual(getProjectFilesStub.callCount, 2);
    assert.ok(await exists(path.join(tmpDir, 's1.cs')));
    assert.ok(await exists(path.join(tmpDir, 's2.cs')));
    assert.ok(await exists(path.join(tmpDir, 's3.cs')));
  });

  test('pull should recursively handle folders with pagination', async () => {
    getProjectFilesStub.withArgs(baseUrl, projectId, undefined, 0, 50).resolves({
      data: [{ id: 'folder-1', name: 'sub', type: 'folder', hasChildren: true }],
      totalCount: 1,
      offset: 0,
      limit: 50,
    });

    getProjectFilesStub.withArgs(baseUrl, projectId, 'folder-1', 0, 50).resolves({
      data: [{ id: 'file-1', name: 'inner.cs', type: 'file', hasChildren: false }],
      totalCount: 1,
      offset: 0,
      limit: 50,
    });

    getScriptContentStub.resolves('inner-content');

    await scriptSyncService.pull(connectionId);

    const content = await fs.readFile(path.join(tmpDir, 'sub', 'inner.cs'), 'utf8');
    assert.strictEqual(content, 'inner-content');
  });

  test('push should update existing remote scripts', async () => {
    await fs.writeFile(path.join(tmpDir, 'script1.cs'), 'local-content');

    getProjectFilesStub.withArgs(baseUrl, projectId, undefined, 0, 1000).resolves({
      data: [{ id: 'remote-1', name: 'script1.cs', type: 'file', hasChildren: false }],
      totalCount: 1,
      offset: 0,
      limit: 1000,
    });

    await scriptSyncService.push(connectionId);

    assert.ok(updateScriptContentStub.calledWith(baseUrl, projectId, 'remote-1', 'local-content'));
    assert.ok(createScriptStub.notCalled);
  });

  test('push should create and then update content for new local scripts', async () => {
    await fs.writeFile(path.join(tmpDir, 'new.js'), 'new-content');

    getProjectFilesStub.withArgs(baseUrl, projectId, undefined, 0, 1000).resolves({
      data: [],
      totalCount: 0,
      offset: 0,
      limit: 1000,
    });

    createScriptStub.resolves({ id: 'new-remote-id', name: 'new.js', path: 'new.js', absolutePath: '', language: 'javascript' });

    await scriptSyncService.push(connectionId);

    assert.ok(createScriptStub.calledWith(baseUrl, projectId, 'new.js', 'javascript'));
    assert.ok(updateScriptContentStub.calledWith(baseUrl, projectId, 'new-remote-id', 'new-content'));
  });

  test('push should handle subdirectories by traversing remote folders', async () => {
    const subDirPath = path.join(tmpDir, 'sub');
    await fs.mkdir(subDirPath);
    await fs.writeFile(path.join(subDirPath, 'sub.cs'), 'sub-content');

    getProjectFilesStub.withArgs(baseUrl, projectId, undefined, 0, 1000).resolves({
      data: [{ id: 'folder-sub-id', name: 'sub', type: 'folder', hasChildren: true }],
      totalCount: 1,
      offset: 0,
      limit: 1000,
    });

    getProjectFilesStub.withArgs(baseUrl, projectId, 'folder-sub-id', 0, 1000).resolves({
      data: [],
      totalCount: 0,
      offset: 0,
      limit: 1000,
    });

    createScriptStub.resolves({ id: 'sub-remote-id', name: 'sub.cs', path: 'sub/sub.cs', absolutePath: '', language: 'csharp' });

    await scriptSyncService.push(connectionId);

    assert.ok(createScriptStub.calledWith(baseUrl, projectId, 'sub/sub.cs', 'csharp'));
    assert.ok(updateScriptContentStub.calledWith(baseUrl, projectId, 'sub-remote-id', 'sub-content'));
  });

  async function exists(filePath: string): Promise<boolean> {
    try {
      await fs.access(filePath);
      return true;
    } catch {
      return false;
    }
  }
});
