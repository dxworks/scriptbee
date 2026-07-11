import { shareAll, withNativeFederation } from '@angular-architects/native-federation/config';

export default withNativeFederation({
  name: 'default-scriptbee-charts',
  exposes: {
    './EChartsChart': './src/app/components/echarts-chart/echarts-chart.ts',
    './BarChart': './src/app/components/bar-chart/bar-chart.ts',
    './TreeMap': './src/app/components/tree-map/tree-map.ts',
    './ScatterPlot': './src/app/components/scatter-plot/scatter-plot.ts',
    './HeatmapChart': './src/app/components/heatmap-chart/heatmap-chart.ts',
  },
  shared: {
    ...shareAll({ singleton: true, strictVersion: false, requiredVersion: 'auto' }),
  },
  skip: ['@angular-architects/native-federation', 'rxjs/ajax', 'rxjs/fetch', 'rxjs/testing', 'rxjs/webSocket'],
  features: {
    ignoreUnusedDeps: true,
  },
});
