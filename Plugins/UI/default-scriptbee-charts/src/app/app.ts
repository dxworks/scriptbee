import { Component, computed, inject } from '@angular/core';
import { BarChart } from './components/bar-chart/bar-chart';
import { ThemeService } from './services/theme.service';
import { BarChartInput, ChartParameters } from './types/ChartInput';

@Component({
  selector: 'app-root',
  imports: [BarChart],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly themeService = inject(ThemeService);

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
}
