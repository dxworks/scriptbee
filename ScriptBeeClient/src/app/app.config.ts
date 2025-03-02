import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  AutoRefreshTokenService,
  createInterceptorCondition,
  INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
  IncludeBearerTokenCondition,
  includeBearerTokenInterceptor,
  provideKeycloak,
  UserActivityService,
  withAutoRefreshToken,
} from 'keycloak-angular';
import { environment } from '../environments/environment';
import { AuthService } from './services/auth/auth.service';
import { MockedAuthService } from './services/auth/mocked-auth.service';
import { IAuthService } from './services/auth/iauth.service';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';

const provideKeycloakAngular = () =>
  provideKeycloak({
    config: {
      url: environment.keycloak.url,
      realm: environment.keycloak.realm,
      clientId: environment.keycloak.clientId,
    },
    initOptions: {
      onLoad: 'check-sso',
      checkLoginIframe: false,
    },
    features: [
      withAutoRefreshToken({
        onInactivityTimeout: 'logout',
        sessionTimeout: 60000,
      }),
    ],
    providers: [AutoRefreshTokenService, UserActivityService],
  });

const provideBearerTokenConfig = () => {
  return {
    provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
    useValue: [
      createInterceptorCondition<IncludeBearerTokenCondition>({
        urlPattern: /.*\/api.*/,
        bearerPrefix: 'Bearer',
      }),
    ],
  };
};

export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: IAuthService,
      useClass: environment.production ? AuthService : MockedAuthService,
    },
    provideKeycloakAngular(),
    provideBearerTokenConfig(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([includeBearerTokenInterceptor])),
    provideMonacoEditor(),
  ],
};
