import { Component, computed, input } from '@angular/core';
import * as echarts from 'echarts/core';
import { ScatterChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { EChartsCoreOption } from 'echarts/types/dist/echarts';
import { ChartParameters, ScatterPlotInput } from '../../types/ChartInput';

echarts.use([ScatterChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

@Component({
  selector: 'app-scatter-plot',
  imports: [NgxEchartsDirective],
  templateUrl: './scatter-plot.html',
  styleUrl: './scatter-plot.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class ScatterPlot {
  parameters = input.required<ChartParameters<ScatterPlotInput>>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.parameters().input;

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'scatter' })),
    };
  });
}
