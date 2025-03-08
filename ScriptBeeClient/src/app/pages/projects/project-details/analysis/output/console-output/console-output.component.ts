import { Component, input } from '@angular/core';

@Component({
  selector: 'app-console-output',
  templateUrl: './console-output.component.html',
  styleUrls: ['./console-output.component.scss'],
})
export class ConsoleOutputComponent {
  content = input.required<string>();
}
