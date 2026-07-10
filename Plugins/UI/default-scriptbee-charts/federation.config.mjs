import { shareAll, withNativeFederation } from '@angular-architects/native-federation/config';

export default withNativeFederation({
  name: 'default-scriptbee-charts',
  exposes: {
    './BarChart': './src/app/components/bar-chart/bar-chart.ts',
    './TreeMap': './src/app/components/tree-map/tree-map.ts',
  },
  shared: {
    ...shareAll({ singleton: true, strictVersion: false, requiredVersion: 'auto' }),
  },
  skip: ['@angular-architects/native-federation', 'rxjs/ajax', 'rxjs/fetch', 'rxjs/testing', 'rxjs/webSocket'],
  features: {
    ignoreUnusedDeps: true,
  },
});
