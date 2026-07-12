import { Component, computed, input } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts';
import { EChartsCoreOption } from 'echarts';
import { EChartsChartInput, Theme } from '../../types/ChartInput';
import * as ecStatImport from 'echarts-stat';

type TransformParam = Parameters<typeof echarts.registerTransform>[0];
interface EcStatModule {
  transform: {
    histogram: TransformParam;
    regression: TransformParam;
    clustering: TransformParam;
  };
}

const ecStat = (ecStatImport as unknown as { default?: EcStatModule } & EcStatModule).default || (ecStatImport as unknown as EcStatModule);

echarts.registerTransform(ecStat.transform.histogram);

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

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();

    return {
      ...(input.options ?? {}),
      series: input.series ?? [],
    };
  });
}
