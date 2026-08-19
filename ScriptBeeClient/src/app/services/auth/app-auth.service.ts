import { inject, Injectable } from '@angular/core';
import { Observable, switchMap } from 'rxjs';
import { AuthService } from './AuthService';
import { AuthConfig, ConfigService } from './config-auth.service';
import { DevAuthService } from './dev-auth.service';
import { OidcAuthService } from './oidc-auth.service';

@Injectable({ providedIn: 'root' })
export class AppAuthService implements AuthService {
  private configService = inject(ConfigService);
  private devAuthService = inject(DevAuthService);
  private oidcAuthService = inject(OidcAuthService);

  isAuthenticated$: Observable<boolean> = this.configService.config$.pipe(
    switchMap((config) => (this.isDevAuth(config) ? this.devAuthService.isAuthenticated$ : this.oidcAuthService.isAuthenticated$))
  );

  userData$: Observable<unknown> = this.configService.config$.pipe(
    switchMap((config) => (this.isDevAuth(config) ? this.devAuthService.userData$ : this.oidcAuthService.userData$))
  );

  checkAuth() {
    return this.configService.config$.pipe(
      switchMap((config) => {
        if (this.isDevAuth(config)) {
          return this.devAuthService.checkAuth();
        } else {
          return this.oidcAuthService.checkAuth();
        }
      })
    );
  }

  login(): void {
    this.configService.config$.subscribe((config) => {
      if (this.isDevAuth(config)) {
        this.devAuthService.login();
      } else {
        this.oidcAuthService.login();
      }
    });
  }

  logout(): void {
    this.configService.config$.subscribe((config) => {
      if (this.isDevAuth(config)) {
        this.devAuthService.logout();
      } else {
        this.oidcAuthService.logout();
      }
    });
  }

  private isDevAuth(config: AuthConfig) {
    return config.authMode === 'Development';
  }
}
