import { Component, computed, input, output } from '@angular/core';
import { EChartsCoreOption } from 'echarts';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts/core';
import { BarChart as EchartsBarChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { BarChartInput, Theme } from '../../types/ChartInput';

echarts.use([EchartsBarChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

@Component({
  selector: 'app-bar-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './bar-chart.html',
  styleUrl: './bar-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class BarChart {
  theme = input.required<Theme>();
  input = input.required<BarChartInput>();
  chartInit = output<echarts.ECharts>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();
    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'bar' })),
    };
  });
}
