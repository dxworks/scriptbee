import { ErrorResponse } from '../types/api';
import { HttpErrorResponse } from '@angular/common/http';

export function convertError(error?: HttpErrorResponse | Error): ErrorResponse | undefined {
  if (!error) {
    return undefined;
  }

  if (error instanceof HttpErrorResponse) {
    return {
      title: error.error?.title ?? 'Unexpected Error Occurred',
      detail: error.error?.detail ?? 'Please try again or contact support.',
      status: error.status,
      errors: error.error?.errors,
    };
  }

  return {
    title: 'Unexpected Error Occurred',
    detail: 'Please try again or contact support.',
    status: 500,
  };
}

export function isNoInstanceAllocatedForProjectError(errorResponse: HttpErrorResponse) {
  const error: ErrorResponse = errorResponse.error;

  return error.title === 'No Instance Allocated For Project';
}
