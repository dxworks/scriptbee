import { Component, input } from '@angular/core';
import { InstanceInfo } from '../../../../../types/instance';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-instance-info',
  templateUrl: './instance-info.component.html',
  styleUrls: ['./instance-info.component.scss'],
  imports: [DatePipe],
})
export class InstanceInfoComponent {
  instanceInfo = input.required<InstanceInfo>();
}
