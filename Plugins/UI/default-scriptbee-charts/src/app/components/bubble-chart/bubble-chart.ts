import { Component, computed, input } from '@angular/core';
import * as echarts from 'echarts/core';
import { ScatterChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { BubbleChartInput, ChartParameters } from '../../types/ChartInput';
import { EChartsCoreOption } from 'echarts';

echarts.use([ScatterChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

@Component({
  selector: 'app-bubble-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './bubble-chart.html',
  styleUrl: './bubble-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class BubbleChart {
  parameters = input.required<ChartParameters<BubbleChartInput>>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.parameters().input;

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'scatter' })),
    };
  });
}
