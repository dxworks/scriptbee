import { ErrorResponse } from '../types/api';

export const DEFAULT_ERROR_RESPONSE: ErrorResponse = {
  title: 'Unexpected Error Occurred',
  detail: 'Please try again or contact support.',
  status: 500,
};
