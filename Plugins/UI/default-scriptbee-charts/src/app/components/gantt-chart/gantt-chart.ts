import { Component, computed, input } from '@angular/core';
import * as echarts from 'echarts/core';
import { CustomChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent, DataZoomComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { GanttChartInput, Theme } from '../../types/ChartInput';
import { EChartsCoreOption, CustomSeriesRenderItemParams, CustomSeriesRenderItemAPI } from 'echarts';

echarts.use([CustomChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent, DataZoomComponent]);

interface GanttSeriesConfig {
  startIndex?: number;
  endIndex?: number;
  categoryIndex?: number;
  startKey?: string;
  endKey?: string;
  categoryKey?: string;
  barHeightRatio?: number;
  borderRadius?: number | number[];
  customStyle?: Record<string, string | number>;
  encode?: Record<string, unknown>;
}

@Component({
  selector: 'app-gantt-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './gantt-chart.html',
  styleUrl: './gantt-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class GanttChart {
  theme = input.required<Theme>();
  input = input.required<GanttChartInput>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => {
        const config = s as typeof s & GanttSeriesConfig;
        const startIndex = config.startIndex ?? 0;
        const endIndex = config.endIndex ?? 1;
        const categoryIndex = config.categoryIndex ?? 2;

        const startKey = config.startKey;
        const endKey = config.endKey;
        const categoryKey = config.categoryKey;

        return {
          ...s,
          type: 'custom',
          encode: config.encode ?? {
            x: startKey !== undefined && endKey !== undefined ? [startKey, endKey] : [startIndex, endIndex],
            y: categoryKey ?? categoryIndex,
          },
          renderItem: (_params: CustomSeriesRenderItemParams, api: CustomSeriesRenderItemAPI) => {
            const startValue = startKey !== undefined ? api.value(startKey) : api.value(startIndex ?? 0);
            const endValue = endKey !== undefined ? api.value(endKey) : api.value(endIndex ?? 1);
            const categoryValue = categoryKey !== undefined ? api.value(categoryKey) : api.value(categoryIndex ?? 2);

            const startCoord = api.coord([startValue, categoryValue]);
            const endCoord = api.coord([endValue, categoryValue]);

            const size = api.size ? api.size([0, 1]) : [0, 20];
            const heightRatio = config.barHeightRatio ?? 0.6;
            const height = (Array.isArray(size) ? size[1] : size) * heightRatio;
            const radius = config.borderRadius ?? 6;

            return {
              type: 'rect',
              transition: ['shape'],
              shape: {
                x: startCoord[0],
                y: startCoord[1] - height / 2,
                width: Math.max(endCoord[0] - startCoord[0], 2),
                height: height,
                r: radius,
              },
              style: {
                fill: api.visual('color'),
                shadowBlur: 4,
                shadowColor: 'rgba(0,0,0,0.1)',
                shadowOffsetX: 0,
                shadowOffsetY: 2,
                ...(config.customStyle ?? {}),
              },
            };
          },
        };
      }),
    };
  });
}
