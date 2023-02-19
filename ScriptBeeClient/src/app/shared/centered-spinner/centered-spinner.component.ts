import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-centered-spinner',
  templateUrl: './centered-spinner.component.html',
  styleUrls: ['./centered-spinner.component.scss'],
})
export class CenteredSpinnerComponent {
  @Input()
  visible = true;
}
