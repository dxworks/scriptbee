import { initFederation } from '@angular-architects/native-federation';

initFederation({ 'default-scriptbee-charts': './remoteEntry.json' })
  .catch((err) => console.error(err))
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  .then((_) => import('./bootstrap'))
  .catch((err) => console.error(err));
