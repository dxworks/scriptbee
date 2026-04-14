import * as assert from 'assert';
import * as sinon from 'sinon';
import * as path from 'path';
import * as fs from 'fs/promises';
import * as os from 'os';
import axiosMockAdapter from 'axios-mock-adapter';
import axiosInstance from '../../../api/axiosInstance';
import { scriptSyncService } from '../../../services/scriptSyncService';
import { connectionService } from '../../../services/connectionService';
import * as workspaceUtils from '../../../utils/workspaceUtils';
import { storage } from '../../../utils/storage';

suite('ScriptSyncService Test Suite', () => {
  let mock: axiosMockAdapter;
  let tmpDir: string;
  let getProjectSrcPathStub: sinon.SinonStub;
  let getConnectionsStub: sinon.SinonStub;
  let getScriptMetaStub: sinon.SinonStub;
  let saveScriptMetaStub: sinon.SinonStub;
  let deleteScriptMetaStub: sinon.SinonStub;

  const baseUrl = 'http://test-url';
  const projectId = 'test-project';
  const connectionId = 'conn-1';

  setup(async () => {
    mock = new axiosMockAdapter(axiosInstance);
    tmpDir = await fs.mkdtemp(path.join(os.tmpdir(), 'sb-test-'));

    getConnectionsStub = sinon.stub(connectionService, 'getConnections');
    getConnectionsStub.resolves([{ id: connectionId, name: 'Test', url: baseUrl, projectId: projectId }]);

    getProjectSrcPathStub = sinon.stub(workspaceUtils, 'getProjectSrcPath');
    getProjectSrcPathStub.returns(tmpDir);

    getScriptMetaStub = sinon.stub(storage, 'getScriptMeta');
    saveScriptMetaStub = sinon.stub(storage, 'saveScriptMeta');
    deleteScriptMetaStub = sinon.stub(storage, 'deleteScriptMeta');
  });

  teardown(async () => {
    mock.restore();
    sinon.restore();
    await fs.rm(tmpDir, { recursive: true, force: true });
  });

  test('pull should handle multi-page pagination and folders', async () => {
    mock.onGet(`/api/projects/${projectId}/files`, { params: { parentId: undefined, offset: 0, limit: 50 } }).reply(200, {
      data: [
        { id: 'f1', name: 's1.cs', type: 'file' },
        { id: 'dir1', name: 'sub', type: 'folder' },
      ],
      totalCount: 3,
    });

    mock.onGet(`/api/projects/${projectId}/files`, { params: { parentId: undefined, offset: 2, limit: 50 } }).reply(200, {
      data: [{ id: 'f3', name: 's3.cs', type: 'file' }],
      totalCount: 3,
    });

    mock.onGet(`/api/projects/${projectId}/files`, { params: { parentId: 'dir1', offset: 0, limit: 50 } }).reply(200, {
      data: [{ id: 'f2', name: 'inner.py', type: 'file' }],
      totalCount: 1,
    });

    mock.onGet(/\/api\/projects\/.*\/scripts\/.*\/content/).reply(200, 'file content');

    await scriptSyncService.pull(connectionId);
    assert.ok(await exists(path.join(tmpDir, 's1.cs')));
  });

  test('push should update existing scripts and handle deletions', async () => {
    await fs.writeFile(path.join(tmpDir, 's1.cs'), 'new content');

    getScriptMetaStub.callsFake(async (uri: any) => {
      if (uri.fsPath.includes('s1.cs')) {
        return { id: 'remote-1', type: 'file' };
      }
      return undefined;
    });

    mock.onGet(`/api/projects/${projectId}/files`).reply(200, {
      data: [
        { id: 'remote-1', name: 's1.cs', type: 'file' },
        { id: 'remote-2', name: 's2.cs', type: 'file' },
      ],
      totalCount: 2,
    });

    mock.onDelete(`/api/projects/${projectId}/files/remote-2`).reply(204);
    mock.onPut(`/api/projects/${projectId}/scripts/remote-1/content`).reply(204);

    await scriptSyncService.push(connectionId);

    const deleteCalls = mock.history.delete.filter((req) => req.url === `/api/projects/${projectId}/files/remote-2`);
    const updateCalls = mock.history.put.filter((req) => req.url === `/api/projects/${projectId}/scripts/remote-1/content`);

    assert.strictEqual(deleteCalls.length, 1);
    assert.strictEqual(updateCalls.length, 1);
  });

  test('push should create new scripts if meta is missing', async () => {
    await fs.writeFile(path.join(tmpDir, 'new.js'), 'console.log()');
    getScriptMetaStub.resolves(undefined);

    mock.onGet(`/api/projects/${projectId}/files`).reply(200, { data: [], totalCount: 0 });

    mock.onPost(`/api/projects/${projectId}/scripts`).reply(201, {
      id: 'new-remote-id',
      name: 'new.js',
      type: 'file',
    });
    mock.onPut(`/api/projects/${projectId}/scripts/new-remote-id/content`).reply(204);

    await scriptSyncService.push(connectionId);

    const createCalls = mock.history.post.filter((req) => req.url === `/api/projects/${projectId}/scripts`);
    const updateCalls = mock.history.put.filter((req) => req.url === `/api/projects/${projectId}/scripts/new-remote-id/content`);

    assert.strictEqual(createCalls.length, 1);
    assert.strictEqual(updateCalls.length, 1);
  });

  async function exists(p: string): Promise<boolean> {
    try {
      await fs.access(p);
      return true;
    } catch {
      return false;
    }
  }
});
