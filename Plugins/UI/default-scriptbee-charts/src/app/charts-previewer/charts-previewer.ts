import { Component, computed, input } from '@angular/core';
import { ChartsPreviewerInput, Theme } from '../types/ChartInput';
import { BarChart } from '../components/bar-chart/bar-chart';
import { EchartsChart } from '../components/echarts-chart/echarts-chart';
import { HeatmapChart } from '../components/heatmap-chart/heatmap-chart';
import { ScatterPlot } from '../components/scatter-plot/scatter-plot';
import { TreeMap } from '../components/tree-map/tree-map';
import { BubbleChart } from '../components/bubble-chart/bubble-chart';

@Component({
  selector: 'app-charts-previewer',
  imports: [BarChart, EchartsChart, HeatmapChart, ScatterPlot, TreeMap, BubbleChart],
  templateUrl: './charts-previewer.html',
  styleUrl: './charts-previewer.scss',
})
export class ChartsPreviewer {
  theme = input<Theme>('light');
  content = input.required<string | ChartsPreviewerInput>();

  input = computed<ChartsPreviewerInput>(() => {
    const rawInput = this.content();
    return typeof rawInput === 'string' ? JSON.parse(rawInput) : rawInput;
  });
}
