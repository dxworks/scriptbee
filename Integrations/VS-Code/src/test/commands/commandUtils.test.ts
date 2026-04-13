import * as assert from 'assert';
import { getConnectionId, CommandConnectionArg } from '../../commands/commandUtils';

suite('getConnectionId()', () => {
  test('should return the string itself when a string is passed', () => {
    const input: CommandConnectionArg = 'conn_abc123';
    const result = getConnectionId(input);

    assert.strictEqual(result, 'conn_abc123');
  });

  test('should return the id from the connection object when a valid object is passed', () => {
    const input: CommandConnectionArg = {
      connection: { id: 'conn_789' },
    };
    const result = getConnectionId(input);

    assert.strictEqual(result, 'conn_789');
  });

  test('should return undefined when the argument is undefined', () => {
    const input: CommandConnectionArg = undefined;
    const result = getConnectionId(input);

    assert.strictEqual(result, undefined);
  });

  test('should return undefined when the object is missing the "connection" key', () => {
    const input = {} as CommandConnectionArg;
    const result = getConnectionId(input);

    assert.strictEqual(result, undefined);
  });

  test('should return undefined when "connection" property is present but undefined', () => {
    const input: CommandConnectionArg = { connection: undefined };
    const result = getConnectionId(input);

    assert.strictEqual(result, undefined);
  });
});
