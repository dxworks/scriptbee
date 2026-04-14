import { Component, computed, effect, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProjectService } from '../../../services/projects/project.service';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../components/error-state/error-state.component';
import { convertError } from '../../../utils/api';
import { ProjectStateService } from '../../../services/projects/project-state.service';
import { of } from 'rxjs';
import { RouterOutlet } from '@angular/router';
import { ProjectLiveUpdatesService } from '../../../services/projects/project-live-updates.service';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details-page.component.html',
  styleUrls: ['./project-details-page.component.scss'],
  providers: [],
  imports: [LoadingProgressBarComponent, ErrorStateComponent, RouterOutlet],
})
export class ProjectDetailsPage {
  projectResource = rxResource({
    params: () => this.projectStateService.currentProjectId(),
    stream: ({ params: id }) => {
      if (!id) {
        return of(null);
      }
      return this.projectService.getProject(id);
    },
  });
  projectResourceError = computed(() => convertError(this.projectResource.error()));

  private projectService = inject(ProjectService);
  private projectStateService = inject(ProjectStateService);
  private projectLiveUpdatesService = inject(ProjectLiveUpdatesService);

  constructor() {
    effect(() => {
      const project = this.projectResource.value();
      if (project) {
        this.projectStateService.currentProject.set(project);
        void this.projectLiveUpdatesService.connect(project.id);
      } else {
        void this.projectLiveUpdatesService.disconnect();
      }
    });
  }
}
