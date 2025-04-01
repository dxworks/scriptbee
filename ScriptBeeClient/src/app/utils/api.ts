import { ErrorResponse } from '../types/api';
import { HttpErrorResponse } from '@angular/common/http';

export const DEFAULT_ERROR_RESPONSE: ErrorResponse = {
  title: 'Unexpected Error Occurred',
  detail: 'Please try again or contact support.',
  status: 500,
};

export function isNoInstanceAllocatedForProjectError(errorResponse: HttpErrorResponse) {
  const error: ErrorResponse = errorResponse.error;

  return error.title === 'No Instance Allocated For Project';
}
