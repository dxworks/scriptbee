import { EChartsCoreOption } from 'echarts/types/dist/echarts';
import { EChartsOption } from 'echarts/types/dist/shared';
import { BarSeriesOption, TreemapSeriesOption } from 'echarts';

export type Theme = 'light' | 'dark';

export interface ChartParameters<T> {
  theme: Theme;
  input: T;
}

interface ChartInputBase {
  xAxis: EChartsOption['xAxis'];
  yAxis: EChartsOption['yAxis'];
  legend: EChartsOption['legend'];
  tooltip: EChartsOption['tooltip'];
  options: EChartsCoreOption;
}

export interface BarChartInput extends Partial<ChartInputBase> {
  series: Omit<BarSeriesOption, 'type'>[];
}

export interface TreeMapInput extends Partial<ChartInputBase> {
  series: Omit<TreemapSeriesOption, 'type'>[];
}
