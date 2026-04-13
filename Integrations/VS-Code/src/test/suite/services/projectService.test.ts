import * as assert from 'assert';
import * as sinon from 'sinon';
import { projectService } from '../../../services/projectService';
import { connectionService } from '../../../services/connectionService';
import { Connection } from '../../../utils/storage';
import * as projectsApi from '../../../api/projects';

suite('ProjectService Test Suite', () => {
  let getConnectionsStub: sinon.SinonStub;
  let updateConnectionStub: sinon.SinonStub;
  let getAllProjectsStub: sinon.SinonStub;

  setup(() => {
    getConnectionsStub = sinon.stub(connectionService, 'getConnections');
    updateConnectionStub = sinon.stub(connectionService, 'updateConnection');
    getAllProjectsStub = sinon.stub(projectsApi, 'getAllProjects');
  });

  teardown(() => {
    sinon.restore();
  });

  test('fetchProjects should pass the URL to the API layer', async () => {
    const url = 'http://test-url';
    await projectService.fetchProjects(url);
    assert.ok(getAllProjectsStub.calledWith(url));
  });

  test('setSelectedProject should update connection by id', async () => {
    const connection: Connection = {
      id: 'conn-1',
      name: 'Local',
      url: 'url-1',
    };
    getConnectionsStub.resolves([connection]);

    await projectService.setSelectedProject('conn-1', 'proj-1');

    assert.strictEqual(connection.projectId, 'proj-1');
    assert.ok(updateConnectionStub.calledWith(connection));
  });

  test('getSelectedProjectId should return from connection by id', async () => {
    const connection: Connection = {
      id: 'conn-1',
      name: 'Local',
      url: 'url-1',
      projectId: 'proj-1',
    };
    getConnectionsStub.resolves([connection]);

    const result = await projectService.getSelectedProjectId('conn-1');
    assert.strictEqual(result, 'proj-1');
  });
});
