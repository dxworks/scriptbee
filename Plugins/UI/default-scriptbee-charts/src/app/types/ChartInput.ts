import { EChartsCoreOption } from 'echarts/types/dist/echarts';
import { BarSeriesOption, ScatterSeriesOption, TreemapSeriesOption } from 'echarts';

export type Theme = 'light' | 'dark';

export interface ChartParameters<T> {
  theme: Theme;
  input: T;
}

export interface BarChartInput {
  series: Omit<BarSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}

export interface TreeMapInput {
  series: Omit<TreemapSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}

export interface ScatterPlotInput {
  series: Omit<ScatterSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}
