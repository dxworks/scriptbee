import '@angular/compiler';
import { HttpRequest } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { describe, expect, it, vi } from 'vitest';
import { AuthService } from '../services/auth/AuthService';
import { authTokenInterceptor } from './auth-token.interceptor';

describe('authTokenInterceptor', () => {
  const runInterceptor = (token: string | null, url = '/api/projects') => {
    const authService = { accessToken$: of(token) } as Partial<AuthService>;
    const next = vi.fn((request) => of(request));

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [{ provide: AuthService, useValue: authService }],
    });

    const request = new HttpRequest('GET', url);
    const result$ = TestBed.runInInjectionContext(() => authTokenInterceptor(request, next));

    return { request, next, result$ };
  };

  it('adds a bearer token to authenticated requests', () => {
    const { next, result$, request } = runInterceptor('test-token');

    result$.subscribe((responseRequest) => {
      const finalRequest = responseRequest as unknown as HttpRequest<unknown>;
      expect(next).toHaveBeenCalledTimes(1);
      expect(finalRequest).not.toBe(request);
      expect(finalRequest.headers.get('Authorization')).toBe('Bearer test-token');
    });
  });

  it('passes the request through when there is no token', () => {
    const { next, result$, request } = runInterceptor(null);

    result$.subscribe((responseRequest) => {
      const finalRequest = responseRequest as unknown as HttpRequest<unknown>;
      expect(next).toHaveBeenCalledTimes(1);
      expect(finalRequest).toBe(request);
      expect(finalRequest.headers.get('Authorization')).toBeNull();
    });
  });

  it.each(['/login', '/oauth', '/connect', '/api/config/auth'])('skips auth for %s', (url) => {
    const { next, result$, request } = runInterceptor('test-token', url);

    result$.subscribe((responseRequest) => {
      const finalRequest = responseRequest as unknown as HttpRequest<unknown>;
      expect(next).toHaveBeenCalledTimes(1);
      expect(finalRequest).toBe(request);
      expect(finalRequest.headers.get('Authorization')).toBeNull();
    });
  });
});
