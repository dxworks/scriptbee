import { Component, Input } from '@angular/core';

export type ApiErrorType = 'error' | 'warning' | 'no-data';

@Component({
  selector: 'app-api-error-message',
  templateUrl: './api-error-message.component.html',
  styleUrls: ['./api-error-message.component.scss']
})
export class ApiErrorMessageComponent {
  @Input()
  message: string;
  @Input()
  type: ApiErrorType = 'error';
}
