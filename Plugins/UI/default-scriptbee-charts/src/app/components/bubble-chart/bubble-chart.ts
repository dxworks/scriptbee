import { Component, computed, input, output } from '@angular/core';
import * as echarts from 'echarts/core';
import { ScatterChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { BubbleChartInput, Theme } from '../../types/ChartInput';
import { EChartsCoreOption } from 'echarts';

echarts.use([ScatterChart, GridComponent, CanvasRenderer, LegendComponent, TooltipComponent]);

type BubbleDataPoint = (number | string)[] | Record<string, number | string>;

interface BubbleSeriesConfig {
  sizeIndex?: number;
  sizeKey?: string;
  sizeMultiplier?: number;
  defaultSize?: number;
}

@Component({
  selector: 'app-bubble-chart',
  imports: [NgxEchartsDirective],
  templateUrl: './bubble-chart.html',
  styleUrl: './bubble-chart.scss',
  providers: [provideEchartsCore({ echarts })],
})
export class BubbleChart {
  theme = input.required<Theme>();
  input = input.required<BubbleChartInput>();
  chartInit = output<echarts.ECharts>();

  options = computed<EChartsCoreOption>(() => {
    const input = this.input();

    return {
      ...(input.options ?? {}),
      series: (input.series ?? []).map((s) => {
        const config = s as typeof s & BubbleSeriesConfig;
        const sizeIndex = config.sizeIndex;
        const sizeKey = config.sizeKey;
        const sizeMultiplier = config.sizeMultiplier ?? 1;
        const defaultSize = config.defaultSize ?? 10;

        return {
          ...s,
          type: 'scatter',
          symbolSize: (data: BubbleDataPoint) => {
            let size: number | string | undefined = defaultSize;

            if (sizeIndex !== undefined && Array.isArray(data)) {
              size = data[sizeIndex];
            } else if (sizeKey !== undefined && typeof data === 'object' && data !== null && !Array.isArray(data)) {
              size = data[sizeKey];
            }

            const numericSize = typeof size === 'number' ? size : Number(size);
            return (isNaN(numericSize) ? defaultSize : numericSize) * sizeMultiplier;
          },
        };
      }),
    };
  });
}
