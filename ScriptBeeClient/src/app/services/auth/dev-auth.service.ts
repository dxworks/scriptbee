import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { AuthService } from './AuthService';
import { LoginResponse } from 'angular-auth-oidc-client';

const accessToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZXYtYWRtaW4iLCJuYW1lIjoiSm9obiBEb2UifQ.Dg75FO7g7sZxek7Wt41_TsR7lcbZJbW4rMOrdExoKrg';

@Injectable({
  providedIn: 'root',
})
export class DevAuthService implements AuthService {
  isAuthenticated$: Observable<boolean> = new BehaviorSubject<boolean>(true).asObservable();
  userData$: Observable<unknown> = new BehaviorSubject(null).asObservable();
  accessToken$: Observable<string | null> = new BehaviorSubject<string | null>(accessToken).asObservable();

  checkAuth(): Observable<LoginResponse> {
    return of({
      isAuthenticated: true,
      userData: null,
      accessToken: accessToken,
      idToken: 'dev-id-token',
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
