import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent implements OnInit {

  module: any;

  constructor() {
  }

  ngOnInit(): void {
    // import('scriptbeeEchartsPlugin/Chart').then(m => {
    //   const chartsModule = m.ChartsModule;
    //
    // })
  }

}
