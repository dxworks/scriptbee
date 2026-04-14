import * as assert from 'assert';
import axiosInstance from '../../api/axiosInstance';
import MockAdapter from 'axios-mock-adapter';
import { getProjectInstances } from '../../api/instances';

suite('Instances API Tests', () => {
  let mock: MockAdapter;

  setup(() => {
    mock = new MockAdapter(axiosInstance);
  });

  teardown(() => {
    mock.restore();
  });

  test('getProjectInstances should return data from the API', async () => {
    const baseUrl = 'http://localhost:5000';
    const projectId = 'test-project';
    const mockResponse = {
      data: [
        { id: 'instance-1', creationDate: '2023-01-01T00:00:00Z', status: 'Loaded' },
        { id: 'instance-2', creationDate: '2023-01-02T00:00:00Z', status: 'Unloaded' },
      ],
    };

    mock.onGet(`${baseUrl}/api/projects/${projectId}/instances`).reply(200, mockResponse);

    const instances = await getProjectInstances(baseUrl, projectId);

    assert.strictEqual(instances.length, 2);
    assert.strictEqual(instances[0].id, 'instance-1');
    assert.strictEqual(instances[1].id, 'instance-2');
  });

  test('getProjectInstances should return empty array on empty response', async () => {
    const baseUrl = 'http://localhost:5000';
    const projectId = 'test-project';

    mock.onGet(`${baseUrl}/api/projects/${projectId}/instances`).reply(200, { data: [] });

    const instances = await getProjectInstances(baseUrl, projectId);

    assert.strictEqual(instances.length, 0);
  });

  test('getProjectInstances should throw on API error', async () => {
    const baseUrl = 'http://localhost:5000';
    const projectId = 'test-project';

    mock.onGet(`${baseUrl}/api/projects/${projectId}/instances`).reply(500);

    await assert.rejects(getProjectInstances(baseUrl, projectId));
  });
});
