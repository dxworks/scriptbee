import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details-page.component.html',
  styleUrls: ['./project-details-page.component.scss'],
  providers: [],
  imports: [MatTabsModule, RouterOutlet],
})
export class ProjectDetailsPage {}
