import { map, Observable } from 'rxjs';
import { OpenIdConfiguration, StsConfigHttpLoader } from 'angular-auth-oidc-client';
import { ConfigService } from '../services/auth/config-auth.service';
import { inject } from '@angular/core';

export const httpLoaderFactory = () => {
  const configService = inject(ConfigService);

  const config$: Observable<OpenIdConfiguration> = configService.config$.pipe(
    map((config) => ({
      authority: config.authority,
      authWellknownEndpointUrl: config.authWellknownEndpointUrl,
      clientId: config.clientId,
      scope: config.scope,
      redirectUrl: window.location.href.split('?')[0],
      postLogoutRedirectUri: window.location.origin,
      responseType: 'code',
      silentRenew: true,
      useRefreshToken: true,
      secureRoutes: ['/api/'],
    }))
  );

  return new StsConfigHttpLoader(config$);
};
