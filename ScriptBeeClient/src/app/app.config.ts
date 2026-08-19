import { ApplicationConfig, inject, provideAppInitializer, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes, withErrorNavigation } from './app.routes';
import { provideHttpClient, withInterceptors, withXsrfConfiguration } from '@angular/common/http';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { clientIdInterceptor } from './utils/client-id.interceptor';
import { GatewayPluginsService } from './services/plugin/gateway-plugins.service';
import { provideAuth, StsConfigLoader } from 'angular-auth-oidc-client';
import { httpLoaderFactory } from './utils/authConfigHttpLoaderFactory';
import { AuthService } from './services/auth/AuthService';
import { AppAuthService } from './services/auth/app-auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideAppInitializer(() => inject(GatewayPluginsService).fetchUIPlugins()),
    provideRouter(routes, withComponentInputBinding(), withErrorNavigation),
    provideHttpClient(
      withInterceptors([clientIdInterceptor]),
      withXsrfConfiguration({
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-XSRF-TOKEN',
      })
    ),
    provideMonacoEditor(),
    provideAuth({
      loader: {
        provide: StsConfigLoader,
        useFactory: httpLoaderFactory,
      },
    }),
    {
      provide: AuthService,
      useClass: AppAuthService,
    },
  ],
};
