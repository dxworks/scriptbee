import * as assert from 'assert';
import axiosMockAdapter from 'axios-mock-adapter';
import axiosInstance from '../../../api/axiosInstance';
import { createScript, getProjectFiles, getScriptContent, updateScriptContent } from '../../../api/projectFiles';

suite('Project Files API Test Suite', () => {
  let mock: axiosMockAdapter;
  const baseUrl = 'http://test-url';
  const projectId = 'test-project';

  setup(() => {
    mock = new axiosMockAdapter(axiosInstance);
  });

  teardown(() => {
    mock.restore();
  });

  test('getProjectFiles should fetch and return paginated files', async () => {
    const parentId = 'folder-1';
    const offset = 10;
    const limit = 20;
    const responseData = {
      data: [{ id: 'file-1', name: 'script.cs', type: 'file', hasChildren: false }],
      totalCount: 100,
      offset: 10,
      limit: 20,
    };

    mock.onGet(`/api/projects/${projectId}/files`).reply((config) => {
      assert.strictEqual(config.baseURL, baseUrl);
      assert.strictEqual(config.params.parentId, parentId);
      assert.strictEqual(config.params.offset, offset);
      assert.strictEqual(config.params.limit, limit);
      return [200, responseData];
    });

    const result = await getProjectFiles(baseUrl, projectId, parentId, offset, limit);
    assert.deepStrictEqual(result, responseData);
  });

  test('createScript should post script data and return new script data', async () => {
    const path = 'folder/new-script.py';
    const language = 'python';
    const responseData = { id: 'new-id', name: 'new-script.py', path, language };

    mock.onPost(`/api/projects/${projectId}/scripts`).reply((config) => {
      const data = JSON.parse(config.data);
      assert.strictEqual(data.path, path);
      assert.strictEqual(data.language, language);
      return [201, responseData];
    });

    const result = await createScript(baseUrl, projectId, path, language);
    assert.deepStrictEqual(result, responseData);
  });

  test('updateScriptContent should put content with text/plain header', async () => {
    const scriptId = 'script-1';
    const content = 'new script content';

    mock.onPut(`/api/projects/${projectId}/scripts/${scriptId}/content`).reply((config) => {
      assert.strictEqual(config.data, content);
      assert.strictEqual(config.headers?.['Content-Type'], 'text/plain');
      return [204];
    });

    await updateScriptContent(baseUrl, projectId, scriptId, content);
  });

  test('getScriptContent should fetch raw script text', async () => {
    const scriptId = 'script-1';
    const content = 'raw script content';

    mock.onGet(`/api/projects/${projectId}/scripts/${scriptId}/content`).reply((config) => {
      assert.strictEqual(config.responseType, 'text');
      return [200, content];
    });

    const result = await getScriptContent(baseUrl, projectId, scriptId);
    assert.strictEqual(result, content);
  });
});
