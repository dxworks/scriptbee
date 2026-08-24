import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { AuthService } from './AuthService';
import { LoginResponse } from 'angular-auth-oidc-client';

@Injectable({
  providedIn: 'root',
})
export class DevAuthService implements AuthService {
  isAuthenticated$: Observable<boolean> = new BehaviorSubject<boolean>(true).asObservable();
  userData$: Observable<unknown> = new BehaviorSubject(null).asObservable();
  accessToken$: Observable<string | null> = new BehaviorSubject<string | null>('dev-access-token').asObservable();

  checkAuth(): Observable<LoginResponse> {
    return of({
      isAuthenticated: true,
      userData: null,
      accessToken: 'dev-access-token',
      idToken: 'dev-id-token',
      refreshToken: 'dev-refresh-token',
    });
  }

  login() {
    console.log('[Dev Mode] Login skipped.');
  }

  logout() {
    console.log('[Dev Mode] Logout clicked.');
    return of();
  }
}
