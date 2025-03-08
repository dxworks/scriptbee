import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Project } from '../../../../../types/project';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-project-information',
  imports: [DatePipe, MatListModule, MatCardModule],
  templateUrl: './project-information.component.html',
  styleUrl: './project-information.component.scss',
})
export class ProjectInformationComponent {
  project = input.required<Project>();
}
