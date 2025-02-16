import { signal, Signal } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { ErrorResponse } from '../types/api';
import { HttpErrorResponse } from '@angular/common/http';
import { DEFAULT_ERROR_RESPONSE } from './api';

export function apiHandler<ActionResult, SuccessResult, Error extends ErrorResponse, ActionParameters = void>(
  action: (parameters: ActionParameters) => Observable<ActionResult>,
  onSuccess?: (data: ActionResult) => SuccessResult | Promise<SuccessResult>
): {
  loading: Signal<boolean>;
  error: Signal<Error | undefined>;
  execute: (parameters: ActionParameters) => void;
} {
  const loading = signal(false);
  const error = signal<Error | undefined>(undefined);

  function execute(parameters: ActionParameters) {
    loading.set(true);
    error.set(undefined);

    action(parameters)
      .pipe(finalize(() => loading.set(false)))
      .subscribe({
        next: (data) => {
          if (!onSuccess) {
            return;
          }
          const result = onSuccess(data);

          if (result instanceof Promise) {
            result.then();
          }
        },
        error: (err: HttpErrorResponse) => {
          error.set(err.error ?? DEFAULT_ERROR_RESPONSE);
        },
      });
  }

  return { loading, error, execute };
}
