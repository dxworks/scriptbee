import * as assert from 'assert';
import * as sinon from 'sinon';
import { connectionService } from '../../../services/connectionService';
import { storage } from '../../../utils/storage';

suite('ConnectionService Test Suite', () => {
  let saveConnectionsStub: sinon.SinonStub;
  let getConnectionsStub: sinon.SinonStub;
  let setActiveConnectionIdStub: sinon.SinonStub;
  let getActiveConnectionIdStub: sinon.SinonStub;

  setup(() => {
    saveConnectionsStub = sinon.stub(storage, 'saveConnections');
    getConnectionsStub = sinon.stub(storage, 'getConnections');
    setActiveConnectionIdStub = sinon.stub(storage, 'setActiveConnectionId');
    getActiveConnectionIdStub = sinon.stub(storage, 'getActiveConnectionId');
  });

  teardown(() => {
    sinon.restore();
  });

  test('addConnection should add a new connection and make it active if it is the first one', async () => {
    getConnectionsStub.resolves([]);
    const name = 'Local';
    const url = 'http://localhost:5000';

    await connectionService.addConnection(name, url);

    assert.ok(saveConnectionsStub.calledOnce);
    const savedConnections = saveConnectionsStub.firstCall.args[0];
    assert.strictEqual(savedConnections.length, 1);
    assert.strictEqual(savedConnections[0].name, name);
    assert.strictEqual(savedConnections[0].url, url);
    assert.ok(setActiveConnectionIdStub.calledWith(savedConnections[0].id));
  });

  test('deleteConnection should remove connection and update active if needed', async () => {
    const connections = [
      {
        id: '1',
        name: 'Local',
        url: '...',
      },
    ];
    getConnectionsStub.resolves(connections);
    getActiveConnectionIdStub.returns('1');

    await connectionService.deleteConnection('1');

    assert.ok(saveConnectionsStub.calledWith([]));
    assert.ok(setActiveConnectionIdStub.calledWith(undefined));
  });
});
