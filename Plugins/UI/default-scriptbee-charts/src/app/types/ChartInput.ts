import { EChartsCoreOption, RegisteredSeriesOption } from 'echarts/types/dist/echarts';

export type Theme = 'light' | 'dark';

type AllSeriesOptions = RegisteredSeriesOption[keyof RegisteredSeriesOption];

export interface ChartsPreviewerInput {
  type: string;
  series: AllSeriesOptions[];
  options?: EChartsCoreOption;
}

export type ChartInput = Omit<ChartsPreviewerInput, 'type'>;

export type EChartsChartInput = ChartInput;
export type BarChartInput = ChartInput;
export type TreeMapInput = ChartInput;
export type ScatterPlotInput = ChartInput;
export type HeatmapChartInput = ChartInput;
