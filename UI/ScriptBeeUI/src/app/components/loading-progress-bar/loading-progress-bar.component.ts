import { Component, input } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-loading-progress-bar',
  imports: [MatProgressBarModule],
  templateUrl: './loading-progress-bar.component.html',
  styleUrl: './loading-progress-bar.component.scss',
})
export class LoadingProgressBarComponent {
  title = input.required<string>();
}
