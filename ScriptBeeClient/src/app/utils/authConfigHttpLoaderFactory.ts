import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { StsConfigHttpLoader } from 'angular-auth-oidc-client';

interface AuthConfig {
  authMode?: string;
  authority: string;
  clientId: string;
  scope: string;
}

export const httpLoaderFactory = (httpClient: HttpClient) => {
  const config$ = httpClient.get<AuthConfig>('/api/config/auth').pipe(
    map((config) => ({
      authority: config.authority,
      clientId: config.clientId,
      scope: config.scope,
      redirectUrl: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
      responseType: 'code',
      silentRenew: true,
      useRefreshToken: true,
      secureRoutes: ['/api/'],
    }))
  );

  return new StsConfigHttpLoader(config$);
};
