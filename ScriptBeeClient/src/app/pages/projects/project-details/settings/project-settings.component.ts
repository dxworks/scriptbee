import { Component, signal } from '@angular/core';
import { ProjectInformationComponent } from './project-information/project-information.component';
import { ProjectDangerZoneComponent } from './project-danger-zone/project-danger-zone.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../../../services/projects/project.service';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { createRxResourceHandler } from '../../../../utils/resource';

@Component({
  selector: 'app-project-settings',
  imports: [ProjectInformationComponent, ProjectDangerZoneComponent, LoadingProgressBarComponent, ErrorStateComponent],
  templateUrl: './project-settings.component.html',
  styleUrl: './project-settings.component.scss',
})
export class ProjectSettingsPage {
  private projectId = signal<string | undefined>(undefined);

  getProjectResource = createRxResourceHandler({
    request: () => this.projectId(),
    loader: (params) => this.projectService.getProject(params.request),
  });

  constructor(
    route: ActivatedRoute,
    private projectService: ProjectService
  ) {
    route.parent?.paramMap.pipe(takeUntilDestroyed()).subscribe({
      next: (paramMap) => {
        this.projectId.set(paramMap.get('id') ?? undefined);
      },
    });
  }
}
