import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { switchMap, take } from 'rxjs';
import { AuthService } from '../services/auth/AuthService';

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  const shouldSkipAuth = req.url.includes('/login') || req.url.includes('/oauth') || req.url.includes('/connect') || req.url.includes('/api/config/auth');

  if (shouldSkipAuth) {
    return next(req);
  }

  return authService.accessToken$.pipe(
    take(1),
    switchMap((token) => {
      if (!token) {
        return next(req);
      }

      const authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });

      return next(authReq);
    })
  );
};
