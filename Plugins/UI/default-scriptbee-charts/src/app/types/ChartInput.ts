import { EChartsCoreOption } from 'echarts/types/dist/echarts';
import { EChartsOption } from 'echarts/types/dist/shared';
import { BarSeriesOption } from 'echarts';

export type Theme = 'light' | 'dark';

export interface ChartParameters<T> {
  theme: Theme;
  input: T;
}

export interface BarChartInput {
  xAxis: EChartsOption['xAxis'];
  yAxis: EChartsOption['yAxis'];
  legend: EChartsOption['legend'];
  tooltip: EChartsOption['tooltip'];
  series: Omit<BarSeriesOption, 'type'>[];
  options?: EChartsCoreOption;
}
