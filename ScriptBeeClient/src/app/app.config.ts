import { ApplicationConfig, inject, provideAppInitializer, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes, withErrorNavigation } from './app.routes';
import { provideHttpClient, withInterceptors, withXsrfConfiguration } from '@angular/common/http';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { clientIdInterceptor } from './utils/client-id.interceptor';
import { GatewayPluginsService } from './services/plugin/gateway-plugins.service';

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
  ],
};
