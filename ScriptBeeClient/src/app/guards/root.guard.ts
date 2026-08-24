import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/AuthService';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const rootGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.isAuthenticated$.pipe(map((isAuth) => router.createUrlTree([isAuth ? '/projects' : '/login'])));
};
