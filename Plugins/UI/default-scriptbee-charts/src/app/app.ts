import { Component, computed, inject, model } from '@angular/core';
import { BarChart } from './components/bar-chart/bar-chart';
import { ThemeService } from './services/theme.service';
import { BarChartInput, ChartParameters, EChartsChartInput, ScatterPlotInput, TreeMapInput } from './types/ChartInput';
import { FormsModule } from '@angular/forms';
import { TreeMap } from './components/tree-map/tree-map';
import { ScatterPlot } from './components/scatter-plot/scatter-plot';
import { EchartsChart } from './components/echarts-chart/echarts-chart';

@Component({
  selector: 'app-root',
  imports: [BarChart, FormsModule, TreeMap, ScatterPlot, EchartsChart],
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

  genericEChartsParameters = computed<ChartParameters<EChartsChartInput>>(() => {
    return {
      theme: this.themeService.echartsTheme(),
      input: {
        options: {
          dataset: [
            {
              source: [
                [8.3, 143],
                [8.6, 214],
                [8.8, 251],
                [10.5, 26],
                [10.7, 86],
                [10.8, 93],
                [11.0, 176],
                [11.0, 39],
                [11.1, 221],
                [11.2, 188],
                [11.3, 57],
                [11.4, 91],
                [11.4, 191],
                [11.7, 8],
                [12.0, 196],
                [12.9, 177],
                [12.9, 153],
                [13.3, 201],
                [13.7, 199],
                [13.8, 47],
                [14.0, 81],
                [14.2, 98],
                [14.5, 121],
                [16.0, 37],
                [16.3, 12],
                [17.3, 105],
                [17.5, 168],
                [17.9, 84],
                [18.0, 197],
                [18.0, 155],
                [20.6, 125],
              ],
            },
            {
              transform: {
                type: 'ecStat:histogram',
                config: {},
              },
            },
            {
              transform: {
                type: 'ecStat:histogram',
                config: { dimensions: [1] },
              },
            },
          ],
          tooltip: {},
          grid: [
            {
              top: '50%',
              right: '50%',
            },
            {
              bottom: '52%',
              right: '50%',
            },
            {
              top: '50%',
              left: '52%',
            },
          ],
          xAxis: [
            {
              scale: true,
              gridIndex: 0,
            },
            {
              type: 'category',
              scale: true,
              axisTick: { show: false },
              axisLabel: { show: false },
              axisLine: { show: false },
              gridIndex: 1,
            },
            {
              scale: true,
              gridIndex: 2,
            },
          ],
          yAxis: [
            {
              gridIndex: 0,
            },
            {
              gridIndex: 1,
            },
            {
              type: 'category',
              axisTick: { show: false },
              axisLabel: { show: false },
              axisLine: { show: false },
              gridIndex: 2,
            },
          ],
        },
        series: [
          {
            name: 'origianl scatter',
            type: 'scatter',
            xAxisIndex: 0,
            yAxisIndex: 0,
            encode: { tooltip: [0, 1] },
            datasetIndex: 0,
          },
          {
            name: 'histogram',
            type: 'bar',
            xAxisIndex: 1,
            yAxisIndex: 1,
            barWidth: '99.3%',
            label: {
              show: true,
              position: 'top',
            },
            encode: { x: 0, y: 1, itemName: 4 },
            datasetIndex: 1,
          },
          {
            name: 'histogram',
            type: 'bar',
            xAxisIndex: 2,
            yAxisIndex: 2,
            barWidth: '99.3%',
            label: {
              show: true,
              position: 'right',
            },
            encode: { x: 1, y: 0, itemName: 4 },
            datasetIndex: 2,
          },
        ],
      },
    };
  });
}
