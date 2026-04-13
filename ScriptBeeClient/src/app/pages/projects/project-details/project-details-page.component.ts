import { Component, computed, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details-page.component.html',
  styleUrls: ['./project-details-page.component.scss'],
  providers: [],
  imports: [RouterOutlet, LoadingProgressBarComponent],
})
export class ProjectDetailsPage {
  private router = inject(Router);

  isNavigating = computed(() => {
    return !!this.router.currentNavigation();
  });
}
