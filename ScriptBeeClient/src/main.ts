import { initFederation } from '@angular-architects/native-federation';

initFederation('/api/plugins/gateway/ui/manifest')
  .then(() => import('./bootstrap'))
  .catch((err) => console.error(err));
