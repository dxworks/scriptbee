import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-console-output',
  templateUrl: './console-output.component.html',
  styleUrls: ['./console-output.component.scss']
})
export class ConsoleOutputComponent implements OnInit {

  @Input()
  consoleOutput: string;

  constructor() { }

  ngOnInit(): void {
  }

}
