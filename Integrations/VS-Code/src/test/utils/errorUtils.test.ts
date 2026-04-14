import * as assert from 'assert';
import { getErrorMessage } from '../../utils/errorUtils';

suite('ErrorUtils Tests', () => {
  test('getErrorMessage should extract message from Error object', () => {
    const error = new Error('Standard Error');
    assert.strictEqual(getErrorMessage(error), 'Standard Error');
  });

  test('getErrorMessage should handle plain strings', () => {
    assert.strictEqual(getErrorMessage('String Error'), 'String Error');
  });

  test('getErrorMessage should extract correct message from AxiosError with response.data.message', () => {
    const axiosError = new Error('Network Error') as any;
    axiosError.isAxiosError = true;
    axiosError.response = {
      data: {
        message: 'Custom Server Error',
      },
    };
    assert.strictEqual(getErrorMessage(axiosError), 'Custom Server Error');
  });

  test('getErrorMessage should extract message from AxiosError with flat response.data string', () => {
    const axiosError = new Error('Network Error') as any;
    axiosError.isAxiosError = true;
    axiosError.response = {
      data: 'Bad Request Payload',
    };
    assert.strictEqual(getErrorMessage(axiosError), 'Bad Request Payload');
  });

  test('getErrorMessage should fallback to native message when response is empty in AxiosError', () => {
    const axiosError = new Error('Network Error') as any;
    axiosError.isAxiosError = true;
    axiosError.response = undefined;
    assert.strictEqual(getErrorMessage(axiosError), 'Network Error');
  });

  test('getErrorMessage should handle null gracefully', () => {
    assert.strictEqual(getErrorMessage(null), 'null');
  });

  test('getErrorMessage should handle undefined gracefully', () => {
    assert.strictEqual(getErrorMessage(undefined), 'undefined');
  });
});
