import { Component, computed, input, output } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts';
import { EChartsCoreOption } from 'echarts';
import { EChartsChartInput, Theme } from '../../types/ChartInput';
import { default as ecStat } from 'echarts-stat';

type TransformParam = Parameters<typeof echarts.registerTransform>[0];
interface EcStatModule {
  transform: {
    histogram: TransformParam;
    regression: TransformParam;
    clustering: TransformParam;
  };
}

echarts.registerTransform((ecStat as unknown as EcStatModule).transform.histogram);

@Component({
  selector: 'app-echarts-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './echarts-chart.html',
  styleUrl: './echarts-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class EchartsChart {
  theme = input.required<Theme>();
  input = input.required<EChartsChartInput>();
  chartInit = output<echarts.ECharts>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();

    return {
      ...(input.options ?? {}),
      series: input.series ?? [],
    };
  });
}
