import { Component, computed, input } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts/core';
import { TreemapChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { ChartParameters, TreeMapInput } from '../../types/ChartInput';
import { EChartsCoreOption } from 'echarts';

echarts.use([TreemapChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

@Component({
  selector: 'app-tree-map',
  imports: [NgxEchartsDirective],
  templateUrl: './tree-map.html',
  styleUrl: './tree-map.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class TreeMap {
  parameters = input.required<ChartParameters<TreeMapInput>>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.parameters().input;

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'treemap' })),
    };
  });
}
