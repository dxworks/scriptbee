import { PactV3 } from '@pact-foundation/pact';
import * as path from 'path';
import { Injectable } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpErrorResponse, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiErrorMessage } from '../src/app/shared/api-error-message';

const PACT_MOCK_SERVER_PORT = 53687;

@Injectable()
class PactInterceptor implements HttpInterceptor {
  intercept(httpRequest: HttpRequest<never>, next: HttpHandler) {
    const request = httpRequest.clone({
      url: `http://localhost:${PACT_MOCK_SERVER_PORT}${httpRequest.url}`,
    });
    return next.handle(request);
  }
}

export function getPactInterceptor() {
  return {
    provide: HTTP_INTERCEPTORS,
    useClass: PactInterceptor,
    multi: true,
  };
}

export function pactWrapper() {
  return new PactV3({
    consumer: 'ScriptBee UI',
    provider: 'ScriptBee API',
    dir: path.resolve(process.cwd(), 'pacts'),
    port: PACT_MOCK_SERVER_PORT,
  });
}

export function getResponseHeaders() {
  return {
    'Content-Type': 'application/json',
  };
}

export function executeSuccessfulInteraction<T>(observable: Observable<T>, assertions?: (data: T) => void): Promise<void> {
  return new Promise<void>((resolve, reject) => {
    observable.subscribe({
      next: (data) => {
        assertions?.(data);
        resolve();
      },
      error: (error) => {
        reject(error);
      },
    });
  });
}

export type ApiHttpErrorResponse = Omit<HttpErrorResponse, 'error'> & { error: ApiErrorMessage };

export function executeFailedInteraction<T>(observable: Observable<T>, assertions: (error: ApiHttpErrorResponse) => void): Promise<void> {
  return new Promise<void>((resolve, reject) => {
    observable.subscribe({
      next: () => {
        reject('Expected error, but got success');
      },
      error: (error) => {
        assertions(error);
        resolve();
      },
    });
  });
}
