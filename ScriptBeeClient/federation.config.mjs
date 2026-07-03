import { withNativeFederation, shareAll } from '@angular-architects/native-federation/config';
export default withNativeFederation({
  name: 'ScriptBeeUI',

  shared: {
    ...shareAll({ singleton: true, strictVersion: false, requiredVersion: 'auto' }),
  },

  skip: ['@angular-architects/native-federation', 'es-module-shims', 'monaco-editor', 'rxjs/ajax', 'rxjs/fetch', 'rxjs/testing', 'rxjs/webSocket'],

  features: {
    ignoreUnusedDeps: true,
  },
});
