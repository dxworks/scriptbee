import { Component, computed, input } from '@angular/core';
import { EChartsCoreOption } from 'echarts';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts/core';
import { GraphChart as EchartsGraphChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { GraphChartInput, Theme } from '../../types/ChartInput';

echarts.use([EchartsGraphChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

@Component({
  selector: 'app-graph-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './graph-chart.html',
  styleUrl: './graph-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class GraphChart {
  theme = input.required<Theme>();
  input = input.required<GraphChartInput>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'graph' })),
    };
  });
}
