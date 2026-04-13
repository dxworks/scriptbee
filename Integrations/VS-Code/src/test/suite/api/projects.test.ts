import * as assert from 'assert';
import axiosMockAdapter from 'axios-mock-adapter';
import axiosInstance from '../../../api/axiosInstance';
import { getAllProjects } from '../../../api/projects';

suite('Projects API Test Suite', () => {
  let mock: axiosMockAdapter;

  setup(() => {
    mock = new axiosMockAdapter(axiosInstance);
  });

  teardown(() => {
    mock.restore();
  });

  test('getAllProjects should return list of projects for a specific URL', async () => {
    const baseUrl = 'http://scriptbee-test:5000';
    const projects = [{ id: '1', name: 'Project 1' }];

    mock.onGet('/api/projects').reply((config) => {
      if (config.baseURL === baseUrl) {
        return [200, { data: projects }];
      }
      return [404];
    });

    const result = await getAllProjects(baseUrl);
    assert.deepStrictEqual(result, projects);
  });

  test('getAllProjects should throw error on failure', async () => {
    mock.onGet('/api/projects').reply(500);

    await assert.rejects(async () => {
      await getAllProjects('http://any-url');
    });
  });
});
