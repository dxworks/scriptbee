import { EChartsCoreOption, RegisteredSeriesOption } from 'echarts/types/dist/echarts';
import { BarSeriesOption, HeatmapSeriesOption, ScatterSeriesOption, TreemapSeriesOption } from 'echarts';

export type Theme = 'light' | 'dark';

export interface ChartParameters<T> {
  theme: Theme;
  input: T;
}

type AllSeriesOptions = RegisteredSeriesOption[keyof RegisteredSeriesOption];

export interface EChartsChartInput {
  series: AllSeriesOptions[];
  options?: EChartsCoreOption;
}

export interface BarChartInput {
  series: Omit<BarSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}

export interface BubbleChartInput {
  series: Omit<ScatterSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}

export interface HeatmapChartInput {
  series: Omit<HeatmapSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}

export interface ScatterPlotInput {
  series: Omit<ScatterSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}

export interface TreeMapInput {
  series: Omit<TreemapSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}
