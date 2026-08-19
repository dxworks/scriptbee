import { inject, Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { map } from 'rxjs';
import { AuthService } from './AuthService';

@Injectable({
  providedIn: 'root',
})
export class OidcAuthService implements AuthService {
  private readonly oidcSecurityService = inject(OidcSecurityService);

  isAuthenticated$ = this.oidcSecurityService.isAuthenticated$.pipe(map((result) => result.isAuthenticated));

  userData$ = this.oidcSecurityService.userData$.pipe(map((result) => result.userData));

  login(): void {
    this.oidcSecurityService.authorize();
  }

  checkAuth() {
    return this.oidcSecurityService.checkAuth();
  }

  logout(): void {
    this.oidcSecurityService.logoff().subscribe();
  }
}
