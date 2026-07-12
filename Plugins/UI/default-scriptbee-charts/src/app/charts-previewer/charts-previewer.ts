import { Component, computed, input, signal } from '@angular/core';
import { ECharts } from 'echarts';
import { ChartsPreviewerInput, Theme } from '../types/ChartInput';
import { BarChart } from '../components/bar-chart/bar-chart';
import { EchartsChart } from '../components/echarts-chart/echarts-chart';
import { HeatmapChart } from '../components/heatmap-chart/heatmap-chart';
import { ScatterPlot } from '../components/scatter-plot/scatter-plot';
import { TreeMap } from '../components/tree-map/tree-map';
import { BubbleChart } from '../components/bubble-chart/bubble-chart';
import { GanttChart } from '../components/gantt-chart/gantt-chart';
import { GraphChart } from '../components/graph-chart/graph-chart';
import { AnalysisFile } from '../types/AnalysisFile';

@Component({
  selector: 'app-charts-previewer',
  imports: [BarChart, EchartsChart, HeatmapChart, ScatterPlot, TreeMap, BubbleChart, GanttChart, GraphChart],
  templateUrl: './charts-previewer.html',
  styleUrl: './charts-previewer.scss',
})
export class ChartsPreviewer {
  theme = input<Theme>('light');
  content = input.required<string | ChartsPreviewerInput>();
  file = input<AnalysisFile>();

  chartInstance = signal<ECharts | null>(null);

  input = computed<ChartsPreviewerInput>(() => {
    const rawInput = this.content();
    return typeof rawInput === 'string' ? JSON.parse(rawInput) : rawInput;
  });

  onChartInit(ec: ECharts) {
    this.chartInstance.set(ec);
  }

  saveAsPNG() {
    const chart = this.chartInstance();
    if (!chart) {
      return;
    }

    const dataUrl = chart.getDataURL({
      type: 'png',
      pixelRatio: 2,
      backgroundColor: '#fff',
    });

    const link = document.createElement('a');
    const fileName = this.file()?.name ?? `${this.input().type ?? ''}-chart`;

    link.download = `${fileName}.png`;
    link.href = dataUrl;
    link.click();
  }
}
