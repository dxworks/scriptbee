import { Component, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { createRxResourceHandler } from '../../../../utils/resource';
import { ProjectService } from '../../../../services/projects/project.service';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { MatDivider } from '@angular/material/divider';
import { UploadModelsComponent } from './upload-models/upload-models.component';
import { LoadModelsComponent } from './load-models/load-models.component';
import { LinkModelsComponent } from './link-models/link-models.component';
import { CurrentlyLoadedModelsComponent } from './currently-loaded-models/currently-loaded-models.component';
import { ProjectContextComponent } from './project-context/project-context.component';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { InstanceService } from '../../../../services/instances/instance.service';
import { CenteredSpinnerComponent } from '../../../../components/centered-spinner/centered-spinner.component';

@Component({
  selector: 'app-project-model-page',
  templateUrl: './project-model-page.component.html',
  styleUrls: ['./project-model-page.component.scss'],
  imports: [
    MatCardModule,
    ErrorStateComponent,
    MatDivider,
    UploadModelsComponent,
    LoadModelsComponent,
    LinkModelsComponent,
    CurrentlyLoadedModelsComponent,
    ProjectContextComponent,
    LoadingProgressBarComponent,
    CenteredSpinnerComponent,
  ],
})
export class ProjectModelPage {
  projectId = signal<string | undefined>(undefined);

  projectResource = createRxResourceHandler({
    request: () => this.projectId(),
    loader: (params) => this.projectService.getProject(params.request),
  });

  currentInstanceInfoResource = createRxResourceHandler({
    request: () => this.projectId(),
    loader: (params) => this.instanceService.getCurrentInstance(params.request),
  });

  constructor(
    route: ActivatedRoute,
    private projectService: ProjectService,
    private instanceService: InstanceService
  ) {
    route.parent?.paramMap.pipe(takeUntilDestroyed()).subscribe({
      next: (paramMap) => {
        this.projectId.set(paramMap.get('id') ?? undefined);
      },
    });
  }
}
