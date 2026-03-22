import { Component, computed, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute } from '@angular/router';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
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
import { InstanceInfoComponent } from './instance-info/instance-info.component';
import { convertError } from '../../../../utils/api';

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
    InstanceInfoComponent,
  ],
})
export class ProjectModelPage {
  projectId = signal<string | undefined>(undefined);

  projectResource = rxResource({
    params: () => this.projectId(),
    stream: ({ params: projectId }) => this.projectService.getProject(projectId),
  });
  projectResourceError = computed(() => convertError(this.projectResource.error()));

  currentInstanceInfoResource = rxResource({
    params: () => this.projectId(),
    stream: ({ params: projectId }) => this.instanceService.getCurrentInstance(projectId),
  });
  currentInstanceInfoResourceError = computed(() => convertError(this.currentInstanceInfoResource.error()));

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
