import * as assert from 'assert';
import * as sinon from 'sinon';
import { projectService } from '../../../services/projectService';
import { connectionService } from '../../../services/connectionService';
import * as projectsApi from '../../../api/projects';
import { Connection } from '../../../utils/storage';

suite('ProjectService Test Suite', () => {
  let getActiveConnectionStub: sinon.SinonStub;
  let updateConnectionStub: sinon.SinonStub;
  let getAllProjectsStub: sinon.SinonStub;

  setup(() => {
    getActiveConnectionStub = sinon.stub(connectionService, 'getActiveConnection');
    updateConnectionStub = sinon.stub(connectionService, 'updateConnection');
    getAllProjectsStub = sinon.stub(projectsApi, 'getAllProjects');
  });

  teardown(() => {
    sinon.restore();
  });

  test('setSelectedProject should update active connection bundle', async () => {
    const connection: Connection = {
      id: '1',
      name: 'Local',
      url: '...',
    };
    getActiveConnectionStub.resolves(connection);

    await projectService.setSelectedProject('proj-1');

    assert.strictEqual(connection.projectId, 'proj-1');
    assert.ok(updateConnectionStub.calledWith(connection));
  });

  test('getSelectedProjectId should return from active connection', async () => {
    const connection = {
      id: '1',
      name: 'Local',
      url: '...',
      projectId: 'proj-1',
    };
    getActiveConnectionStub.resolves(connection);

    const result = await projectService.getSelectedProjectId();
    assert.strictEqual(result, 'proj-1');
  });
});
