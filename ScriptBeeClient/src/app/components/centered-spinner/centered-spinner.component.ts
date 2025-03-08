import { Component, input } from '@angular/core';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-centered-spinner',
  templateUrl: './centered-spinner.component.html',
  styleUrls: ['./centered-spinner.component.scss'],
  imports: [MatProgressSpinner],
})
export class CenteredSpinnerComponent {
  text = input.required<string>();
}
