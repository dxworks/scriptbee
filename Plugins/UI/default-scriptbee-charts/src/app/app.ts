import { Component, computed, inject, model } from '@angular/core';
import { BarChart } from './components/bar-chart/bar-chart';
import { ThemeService } from './services/theme.service';
import { BarChartInput, ChartParameters, TreeMapInput } from './types/ChartInput';
import { FormsModule } from '@angular/forms';
import { TreeMap } from './components/tree-map/tree-map';

@Component({
  selector: 'app-root',
  imports: [BarChart, FormsModule, TreeMap],
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
        series: series,
        options: {
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
}
