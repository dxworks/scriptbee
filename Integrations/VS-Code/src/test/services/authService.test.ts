import * as assert from 'assert';
import * as sinon from 'sinon';
import axios from 'axios';
import MockAdapter from 'axios-mock-adapter';
import { AuthService, authService, WebAuthConfig } from '../../services/authService';
import { storage } from '../../utils/storage';

suite('AuthService Tests', () => {
  let axiosMock: MockAdapter;
  let getSecretStub: sinon.SinonStub;
  let storeSecretStub: sinon.SinonStub;
  let deleteSecretStub: sinon.SinonStub;

  setup(() => {
    axiosMock = new MockAdapter(axios);
    getSecretStub = sinon.stub(storage, 'getSecret');
    storeSecretStub = sinon.stub(storage, 'storeSecret').resolves();
    deleteSecretStub = sinon.stub(storage, 'deleteSecret').resolves();
  });

  teardown(() => {
    axiosMock.restore();
    sinon.restore();
  });

  test('generateCodeVerifier should return high-entropy base64url string', () => {
    const service = new AuthService();
    const verifier1 = service.generateCodeVerifier();
    const verifier2 = service.generateCodeVerifier();

    assert.ok(verifier1.length >= 43);
    assert.notStrictEqual(verifier1, verifier2);
  });

  test('generateCodeChallenge should produce deterministic sha256 base64url hash', () => {
    const service = new AuthService();
    const verifier = 'test-verifier-value';
    const challenge1 = service.generateCodeChallenge(verifier);
    const challenge2 = service.generateCodeChallenge(verifier);

    assert.ok(challenge1);
    assert.strictEqual(challenge1, challenge2);
  });

  test('getAuthConfig should fetch config from server', async () => {
    const mockConfig: WebAuthConfig = {
      authMode: 'OIDC',
      authority: 'http://keycloak:8080/realms/scriptbee',
      clientId: 'scriptbee-vscode',
      scope: 'openid profile',
    };

    axiosMock.onGet('http://localhost:5000/api/config/auth').reply(200, mockConfig);

    const config = await authService.getAuthConfig('http://localhost:5000');

    assert.deepStrictEqual(config, mockConfig);
  });

  test('getAuthConfig should return null on error', async () => {
    axiosMock.onGet('http://localhost:5000/api/config/auth').reply(500);

    const config = await authService.getAuthConfig('http://localhost:5000');

    assert.strictEqual(config, null);
  });

  test('getOpenIdConfiguration should fetch discovery document', async () => {
    const discoveryResponse = {
      authorization_endpoint: 'http://auth/authorize',
      token_endpoint: 'http://auth/token',
    };

    axiosMock.onGet('http://keycloak:8080/realms/sb/.well-known/openid-configuration').reply(200, discoveryResponse);

    const openIdConfig = await authService.getOpenIdConfiguration({
      authority: 'http://keycloak:8080/realms/sb',
    });

    assert.deepStrictEqual(openIdConfig, discoveryResponse);
  });

  test('getAccessToken should return stored unexpired token', async () => {
    const tokenData = {
      accessToken: 'valid-access-token',
      expiresAt: Date.now() + 300000,
    };
    getSecretStub.resolves(JSON.stringify(tokenData));

    const token = await authService.getAccessToken({ id: 'conn-1', name: 'Conn', url: 'http://localhost:5000' });

    assert.strictEqual(token, 'valid-access-token');
  });

  test('getAccessToken should refresh token when expired', async () => {
    const expiredTokenData = {
      accessToken: 'expired-access-token',
      refreshToken: 'valid-refresh-token',
      expiresAt: Date.now() - 10000,
    };
    getSecretStub.resolves(JSON.stringify(expiredTokenData));

    axiosMock.onGet('http://localhost:5000/api/config/auth').reply(200, {
      authMode: 'OIDC',
      authority: 'http://auth.example.com',
      clientId: 'vscode-client',
    });

    axiosMock.onGet('http://auth.example.com/.well-known/openid-configuration').reply(200, {
      authorization_endpoint: 'http://auth.example.com/auth',
      token_endpoint: 'http://auth.example.com/token',
    });

    axiosMock.onPost('http://auth.example.com/token').reply(200, {
      access_token: 'new-access-token',
      refresh_token: 'new-refresh-token',
      expires_in: 3600,
    });

    const token = await authService.getAccessToken({ id: 'conn-1', name: 'Conn', url: 'http://localhost:5000' });

    assert.strictEqual(token, 'new-access-token');
    assert.strictEqual(storeSecretStub.calledOnce, true);
  });

  test('setAccessToken should store bearer token in secret storage', async () => {
    await authService.setAccessToken('conn-1', 'manual-token');

    assert.strictEqual(storeSecretStub.calledOnce, true);
    assert.strictEqual(storeSecretStub.firstCall.args[0], 'scriptbee.auth.token.conn-1');
    const savedData = JSON.parse(storeSecretStub.firstCall.args[1] as string);
    assert.strictEqual(savedData.accessToken, 'manual-token');
  });

  test('logout should delete token from secret storage', async () => {
    await authService.logout('conn-1');

    assert.strictEqual(deleteSecretStub.calledOnce, true);
    assert.strictEqual(deleteSecretStub.firstCall.args[0], 'scriptbee.auth.token.conn-1');
  });

  test('isAuthenticated should return true when token exists', async () => {
    getSecretStub.resolves(JSON.stringify({ accessToken: 'valid-token' }));

    const authenticated = await authService.isAuthenticated('conn-1');

    assert.strictEqual(authenticated, true);
  });

  test('isAuthenticated should return false when no token stored', async () => {
    getSecretStub.resolves(undefined);

    const authenticated = await authService.isAuthenticated('conn-1');

    assert.strictEqual(authenticated, false);
  });
});
