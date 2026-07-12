import {Component, computed, inject, model} from '@angular/core';
import {ThemeService} from './services/theme.service';
import {ChartsPreviewerInput} from './types/ChartInput';
import {FormsModule} from '@angular/forms';
import {ChartsPreviewer} from './charts-previewer/charts-previewer';

@Component({
  selector: 'app-root',
  imports: [FormsModule, ChartsPreviewer],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly themeService = inject(ThemeService);

  selectedChartType = model<string>('bar');

  barChartParameters = computed<ChartsPreviewerInput>(() => {
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

    const series: ChartsPreviewerInput['series'] = [
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
      type: 'bar',
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
    };
  });

  bubbleChartParameters = computed<ChartsPreviewerInput>(() => {
    const data = [
      [
        [28604, 77, 17096869, 'Australia', 1990],
        [31163, 77.4, 27662440, 'Canada', 1990],
      ],
      [
        [44056, 81.8, 23968973, 'Australia', 2015],
        [43294, 81.7, 35939927, 'Canada', 2015],
      ],
    ];

    return {
      options: {
        title: {
          text: 'Life Expectancy and GDP by Country',
          left: '5%',
          top: '3%',
        },
        legend: {
          right: '10%',
          top: '3%',
          data: ['1990', '2015'],
        },
        grid: {
          left: '8%',
          top: '10%',
        },
        xAxis: {
          splitLine: {
            lineStyle: {
              type: 'dashed',
            },
          },
        },
        yAxis: {
          splitLine: {
            lineStyle: {
              type: 'dashed',
            },
          },
          scale: true,
        },
      },
      series: [
        {
          name: '1990',
          data: data[0],
          symbolSize: [50, 24, 70],
        },
        {
          name: '2015',
          data: data[1],
          symbolSize: 60,
        },
      ],
    };
  });

  treeMapParameters = computed<ChartsPreviewerInput>(() => {
    const series: ChartsPreviewerInput['series'] = [
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
      type: 'tree-map',
      series: series,
      options: {
        tooltip: {},
      },
    };
  });

  scatterPlotParameters = computed<ChartsPreviewerInput>(() => {
    return {
      type: 'scatter-plot',
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
    };
  });

  heatmapChartParameters = computed<ChartsPreviewerInput>(() => {
    const hours = [
      '12a',
      '1a',
      '2a',
      '3a',
      '4a',
      '5a',
      '6a',
      '7a',
      '8a',
      '9a',
      '10a',
      '11a',
      '12p',
      '1p',
      '2p',
      '3p',
      '4p',
      '5p',
      '6p',
      '7p',
      '8p',
      '9p',
      '10p',
      '11p',
    ];

    const days = ['Saturday', 'Friday', 'Thursday', 'Wednesday', 'Tuesday', 'Monday', 'Sunday'];

    const data = [
      [0, 0, 5],
      [0, 1, 1],
      [0, 2, 0],
      [0, 3, 0],
      [0, 4, 0],
      [0, 5, 0],
      [0, 6, 0],
      [0, 7, 0],
      [0, 8, 0],
      [0, 9, 0],
      [0, 10, 0],
      [0, 11, 2],
      [0, 12, 4],
      [0, 13, 1],
      [0, 14, 1],
      [0, 15, 3],
      [0, 16, 4],
      [0, 17, 6],
      [0, 18, 4],
      [0, 19, 4],
      [0, 20, 3],
      [0, 21, 3],
      [0, 22, 2],
      [0, 23, 5],
      [1, 0, 7],
      [1, 1, 0],
      [1, 2, 0],
      [1, 3, 0],
      [1, 4, 0],
      [1, 5, 0],
      [1, 6, 0],
      [1, 7, 0],
      [1, 8, 0],
      [1, 9, 0],
      [1, 10, 5],
      [1, 11, 2],
      [1, 12, 2],
      [1, 13, 6],
      [1, 14, 9],
      [1, 15, 11],
      [1, 16, 6],
      [1, 17, 7],
      [1, 18, 8],
      [1, 19, 12],
      [1, 20, 5],
      [1, 21, 5],
      [1, 22, 7],
      [1, 23, 2],
      [2, 0, 1],
      [2, 1, 1],
      [2, 2, 0],
      [2, 3, 0],
      [2, 4, 0],
      [2, 5, 0],
      [2, 6, 0],
      [2, 7, 0],
      [2, 8, 0],
      [2, 9, 0],
      [2, 10, 3],
      [2, 11, 2],
      [2, 12, 1],
      [2, 13, 9],
      [2, 14, 8],
      [2, 15, 10],
      [2, 16, 6],
      [2, 17, 5],
      [2, 18, 5],
      [2, 19, 5],
      [2, 20, 7],
      [2, 21, 4],
      [2, 22, 2],
      [2, 23, 4],
      [3, 0, 7],
      [3, 1, 3],
      [3, 2, 0],
      [3, 3, 0],
      [3, 4, 0],
      [3, 5, 0],
      [3, 6, 0],
      [3, 7, 0],
      [3, 8, 1],
      [3, 9, 0],
      [3, 10, 5],
      [3, 11, 4],
      [3, 12, 7],
      [3, 13, 14],
      [3, 14, 13],
      [3, 15, 12],
      [3, 16, 9],
      [3, 17, 5],
      [3, 18, 5],
      [3, 19, 10],
      [3, 20, 6],
      [3, 21, 4],
      [3, 22, 4],
      [3, 23, 1],
      [4, 0, 1],
      [4, 1, 3],
      [4, 2, 0],
      [4, 3, 0],
      [4, 4, 0],
      [4, 5, 1],
      [4, 6, 0],
      [4, 7, 0],
      [4, 8, 0],
      [4, 9, 2],
      [4, 10, 4],
      [4, 11, 4],
      [4, 12, 2],
      [4, 13, 4],
      [4, 14, 4],
      [4, 15, 14],
      [4, 16, 12],
      [4, 17, 1],
      [4, 18, 8],
      [4, 19, 5],
      [4, 20, 3],
      [4, 21, 7],
      [4, 22, 3],
      [4, 23, 0],
      [5, 0, 2],
      [5, 1, 1],
      [5, 2, 0],
      [5, 3, 3],
      [5, 4, 0],
      [5, 5, 0],
      [5, 6, 0],
      [5, 7, 0],
      [5, 8, 2],
      [5, 9, 0],
      [5, 10, 4],
      [5, 11, 1],
      [5, 12, 5],
      [5, 13, 10],
      [5, 14, 5],
      [5, 15, 7],
      [5, 16, 11],
      [5, 17, 6],
      [5, 18, 0],
      [5, 19, 5],
      [5, 20, 3],
      [5, 21, 4],
      [5, 22, 2],
      [5, 23, 0],
      [6, 0, 1],
      [6, 1, 0],
      [6, 2, 0],
      [6, 3, 0],
      [6, 4, 0],
      [6, 5, 0],
      [6, 6, 0],
      [6, 7, 0],
      [6, 8, 0],
      [6, 9, 0],
      [6, 10, 1],
      [6, 11, 0],
      [6, 12, 2],
      [6, 13, 1],
      [6, 14, 3],
      [6, 15, 4],
      [6, 16, 0],
      [6, 17, 0],
      [6, 18, 0],
      [6, 19, 0],
      [6, 20, 1],
      [6, 21, 2],
      [6, 22, 2],
      [6, 23, 6],
    ].map(function (item) {
      return [item[1], item[0], item[2] || '-'];
    });

    return {
      type: 'heatmap',
      options: {
        tooltip: {
          position: 'top',
        },
        grid: {
          height: '50%',
          top: '10%',
        },
        xAxis: {
          type: 'category',
          data: hours,
          splitArea: {
            show: true,
          },
        },
        yAxis: {
          type: 'category',
          data: days,
          splitArea: {
            show: true,
          },
        },
        visualMap: {
          min: 0,
          max: 10,
          calculable: true,
          orient: 'horizontal',
          left: 'center',
          bottom: '15%',
        },
      },
      series: [
        {
          name: 'Punch Card',
          data: data,
          label: {
            show: true,
          },
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowColor: 'rgba(0, 0, 0, 0.5)',
            },
          },
        },
      ],
    };
  });

  genericEChartsParameters = computed<ChartsPreviewerInput>(() => {
    return {
      type: 'echarts',
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
              config: {dimensions: [1]},
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
            axisTick: {show: false},
            axisLabel: {show: false},
            axisLine: {show: false},
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
            axisTick: {show: false},
            axisLabel: {show: false},
            axisLine: {show: false},
            gridIndex: 2,
          },
        ],
      },
      series: [
        {
          name: 'original scatter',
          type: 'scatter',
          xAxisIndex: 0,
          yAxisIndex: 0,
          encode: {tooltip: [0, 1]},
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
          encode: {x: 0, y: 1, itemName: 4},
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
          encode: {x: 1, y: 0, itemName: 4},
          datasetIndex: 2,
        },
      ],
    };
  });
}
