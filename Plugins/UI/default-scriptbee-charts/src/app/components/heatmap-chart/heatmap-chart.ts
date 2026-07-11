import { Component, computed, input } from '@angular/core';
import * as echarts from 'echarts/core';
import { HeatmapChart as HeatMapComponent } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { ChartParameters, HeatmapChartInput } from '../../types/ChartInput';
import { EChartsCoreOption } from 'echarts';

echarts.use([HeatMapComponent, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

@Component({
  selector: 'app-heatmap-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './heatmap-chart.html',
  styleUrl: './heatmap-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class HeatmapChart {
  parameters = input.required<ChartParameters<HeatmapChartInput>>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.parameters().input;

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'heatmap' })),
    };
  });
}
