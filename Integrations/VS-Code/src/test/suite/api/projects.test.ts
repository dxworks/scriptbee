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

  test('getAllProjects should return list of projects', async () => {
    const projects = [
      {
        id: '1',
        name: 'Project 1',
      },
    ];
    mock.onGet('/api/projects').reply(200, {
      data: projects,
    });

    const result = await getAllProjects();
    assert.deepStrictEqual(result, projects);
  });

  test('getAllProjects should throw error on failure', async () => {
    mock.onGet('/api/projects').reply(500);

    await assert.rejects(async () => {
      await getAllProjects();
    });
  });
});
