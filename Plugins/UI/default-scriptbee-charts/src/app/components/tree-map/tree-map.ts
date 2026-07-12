import { Component, computed, input, output } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts/core';
import { TreemapChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { Theme, TreeMapInput } from '../../types/ChartInput';
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
  theme = input.required<Theme>();
  input = input.required<TreeMapInput>();
  chartInit = output<echarts.ECharts>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => ({ ...s, type: 'treemap' })),
    };
  });
}
