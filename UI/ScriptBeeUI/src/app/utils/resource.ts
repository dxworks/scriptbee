import { computed, ResourceRef, Signal } from '@angular/core';
import { rxResource, RxResourceOptions } from '@angular/core/rxjs-interop';
import { ErrorResponse } from '../types/api';
import { HttpErrorResponse } from '@angular/common/http';
import { DEFAULT_ERROR_RESPONSE } from './api';

export interface ResourceRefWithErrors<T, E> extends ResourceRef<T> {
  readonly error: Signal<E>;
}

export function createRxResourceHandler<T, R, E extends ErrorResponse>(options: RxResourceOptions<T, R>): ResourceRefWithErrors<T | undefined, E | undefined> {
  return resourceHandler(rxResource(options));
}

function resourceHandler<T, E extends ErrorResponse>(res: ResourceRef<T | undefined>): ResourceRefWithErrors<T | undefined, E | undefined> {
  const signalError = computed<E | undefined>(() => {
    const error = res.error() as HttpErrorResponse | undefined;

    if (!error) {
      return undefined;
    }

    return error.error ?? DEFAULT_ERROR_RESPONSE;
  });

  return {
    ...res,
    error: signalError,
  };
}
