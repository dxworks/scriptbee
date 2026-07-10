import { Component, computed, inject, model } from '@angular/core';
import { BarChart } from './components/bar-chart/bar-chart';
import { ThemeService } from './services/theme.service';
import { BarChartInput, ChartParameters, ScatterPlotInput, TreeMapInput } from './types/ChartInput';
import { FormsModule } from '@angular/forms';
import { TreeMap } from './components/tree-map/tree-map';
import { ScatterPlot } from './components/scatter-plot/scatter-plot';

@Component({
  selector: 'app-root',
  imports: [BarChart, FormsModule, TreeMap, ScatterPlot],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly themeService = inject(ThemeService);

  selectedChartType = model<string>('bar');

  barChartParameters = computed<ChartParameters<BarChartInput>>(() => {
    const xAxisData = [];
    const data1 = [];
    const data2 = [];
    const data3 = [];

    for (let i = 0; i < 100; i++) {
      xAxisData.push('category' + i);
      data1.push((Math.sin(i / 5) * (i / 5 - 10) + i / 6) * 5);
      data2.push((Math.cos(i / 5) * (i / 5 - 10) + i / 6) * 5);
      data3.push((Math.sin(i / 5) * (i / 5 + 10) + i / 6) * 5);
    }

    const series: ChartParameters<BarChartInput>['input']['series'] = [
      {
        name: 'bar',
        data: data1,
      },
      {
        name: 'bar2',
        data: data2,
      },
      {
        name: 'bar3',
        data: data3,
        color: 'red',
      },
    ];

    return {
      theme: this.themeService.echartsTheme(),
      input: {
        series: series,
        options: {
          xAxis: {
            data: xAxisData,
            silent: false,
            splitLine: {
              show: false,
            },
          },
          yAxis: {},
          legend: {
            data: ['bar', 'bar2', 'bar3'],
            align: 'left',
          },
          tooltip: {},
          animationEasing: 'elasticOut',
        },
      },
    };
  });

  treeMapParameters = computed<ChartParameters<TreeMapInput>>(() => {
    const series: ChartParameters<TreeMapInput>['input']['series'] = [
      {
        name: 'bar',
        data: [
          {
            name: 'nodeA',
            value: 10,
            children: [
              {
                name: 'nodeAa',
                value: 9,
              },
              {
                name: 'nodeAb',
                value: 1,
              },
            ],
          },
          {
            name: 'nodeB',
            value: 20,
            children: [
              {
                name: 'nodeBa',
                value: 20,
                children: [
                  {
                    name: 'nodeBa1',
                    value: 20,
                  },
                ],
              },
            ],
          },
        ],
      },
    ];

    return {
      theme: this.themeService.echartsTheme(),
      input: {
        series: series,
      },
    };
  });

  scatterPlotParameters = computed<ChartParameters<ScatterPlotInput>>(() => {
    return {
      theme: this.themeService.echartsTheme(),
      input: {
        series: [
          {
            symbolSize: 20,
            data: [
              [10.0, 8.04],
              [8.07, 6.95],
              [13.0, 7.58],
              [9.05, 8.81],
              [11.0, 8.33],
              [14.0, 7.66],
              [13.4, 6.81],
              [10.0, 6.33],
              [14.0, 8.96],
              [12.5, 6.82],
              [9.15, 7.2],
              [11.5, 7.2],
              [3.03, 4.23],
              [12.2, 7.83],
              [2.02, 4.47],
              [1.05, 3.33],
              [4.05, 4.96],
              [6.03, 7.24],
              [12.0, 6.26],
              [12.0, 8.84],
              [7.08, 5.82],
              [5.02, 5.68],
            ],
          },
        ],
        options: {
          xAxis: {},
          yAxis: {},
        },
      },
    };
  });
}
