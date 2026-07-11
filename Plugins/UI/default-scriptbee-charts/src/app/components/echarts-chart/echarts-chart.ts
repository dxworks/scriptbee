import { Component, computed, input } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts';
import { EChartsCoreOption } from 'echarts';
import { ChartParameters, EChartsChartInput } from '../../types/ChartInput';
import * as ecStat from 'echarts-stat';

// @ts-expect-error update when types ever available
echarts.registerTransform(ecStat.transform.histogram);

@Component({
  selector: 'app-echarts-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './echarts-chart.html',
  styleUrl: './echarts-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class EchartsChart {
  parameters = input.required<ChartParameters<EChartsChartInput>>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.parameters().input;

    return {
      ...(input.options ?? {}),
      series: input.series ?? [],
    };
  });
}
