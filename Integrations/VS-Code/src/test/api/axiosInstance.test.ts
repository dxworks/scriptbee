import * as assert from 'assert';
import * as sinon from 'sinon';
import MockAdapter from 'axios-mock-adapter';
import axiosInstance from '../../api/axiosInstance';
import { connectionService } from '../../services/connectionService';
import { authService } from '../../services/authService';
import { ClientIdService } from '../../services/clientIdService';

suite('AxiosInstance Interceptor Tests', () => {
  let mock: MockAdapter;
  let getActiveConnectionStub: sinon.SinonStub;
  let getAccessTokenStub: sinon.SinonStub;

  setup(() => {
    mock = new MockAdapter(axiosInstance);
    getActiveConnectionStub = sinon.stub(connectionService, 'getActiveConnection');
    getAccessTokenStub = sinon.stub(authService, 'getAccessToken');
  });

  teardown(() => {
    mock.restore();
    sinon.restore();
  });

  test('should attach X-Client-Id header and Authorization Bearer header when token is present', async () => {
    const connection = { id: 'conn-1', name: 'Local', url: 'http://localhost:5000' };
    getActiveConnectionStub.resolves(connection);
    getAccessTokenStub.resolves('test-jwt-token');

    let interceptedHeaders: Record<string, string | undefined> | undefined;
    mock.onGet('http://localhost:5000/api/projects').reply((config) => {
      interceptedHeaders = config.headers as Record<string, string | undefined>;
      return [200, []];
    });

    await axiosInstance.get('http://localhost:5000/api/projects');

    assert.ok(interceptedHeaders);
    assert.strictEqual(interceptedHeaders['X-Client-Id'], ClientIdService.clientId);
    assert.strictEqual(interceptedHeaders['Authorization'], 'Bearer test-jwt-token');
  });

  test('should attach X-Client-Id without Authorization header when not authenticated', async () => {
    getActiveConnectionStub.resolves(undefined);
    getAccessTokenStub.resolves(undefined);

    let interceptedHeaders: Record<string, string | undefined> | undefined;
    mock.onGet('http://localhost:5000/api/projects').reply((config) => {
      interceptedHeaders = config.headers as Record<string, string | undefined>;
      return [200, []];
    });

    await axiosInstance.get('http://localhost:5000/api/projects');

    assert.ok(interceptedHeaders);
    assert.strictEqual(interceptedHeaders['X-Client-Id'], ClientIdService.clientId);
    assert.strictEqual(interceptedHeaders['Authorization'], undefined);
  });
});
