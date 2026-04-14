import * as assert from 'assert';
import { ClientIdService } from '../../services/clientIdService';

suite('ClientIdService Tests', () => {
  test('clientId should be a non-empty string', () => {
    assert.ok(ClientIdService.clientId);
    assert.strictEqual(typeof ClientIdService.clientId, 'string');
  });

  test('clientId should look like a UUID', () => {
    const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    assert.ok(uuidRegex.test(ClientIdService.clientId));
  });

  test('clientId should be stable across calls (singleton behavior)', () => {
    const id1 = ClientIdService.clientId;
    const id2 = ClientIdService.clientId;
    assert.strictEqual(id1, id2);
  });
});
