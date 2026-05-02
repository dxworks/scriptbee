import { initFederation } from '@angular-architects/native-federation';

initFederation('/api/plugins/gateway/ui/manifest')
  .catch((err) => console.error(err))
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  .then((_) => import('./bootstrap'))
  .catch((err) => console.error(err));
