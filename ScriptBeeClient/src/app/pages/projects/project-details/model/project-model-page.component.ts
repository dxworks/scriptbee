import { Component, computed, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProjectService } from '../../../../services/projects/project.service';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { MatDivider } from '@angular/material/divider';
import { UploadModelsComponent } from './upload-models/upload-models.component';
import { LoadModelsComponent } from './load-models/load-models.component';
import { LinkModelsComponent } from './link-models/link-models.component';
import { CurrentlyLoadedModelsComponent } from './currently-loaded-models/currently-loaded-models.component';
import { ProjectContextComponent } from './project-context/project-context.component';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { InstanceInfoComponent } from './instance-info/instance-info.component';
import { convertError } from '../../../../utils/api';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { of } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { InstanceNotAllocatedDialog } from '../../../../components/dialogs/instance-not-allocated-dialog/instance-not-allocated-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatButton } from '@angular/material/button';
import { InstanceAllocationService } from '../../../../services/instances/instance-allocation.service';

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
    InstanceInfoComponent,
    MatIconModule,
    MatButton,
  ],
})
export class ProjectModelPage {
  private projectStateService = inject(ProjectStateService);
  private projectService = inject(ProjectService);
  private instanceAllocationService = inject(InstanceAllocationService);
  private dialog = inject(MatDialog);

  projectId = computed(() => this.projectStateService.currentProjectId());

  projectResource = rxResource({
    params: () => this.projectId(),
    stream: ({ params: projectId }) => (projectId ? this.projectService.getProject(projectId) : of(undefined)),
  });
  projectResourceError = computed(() => convertError(this.projectResource.error()));

  currentInstanceResource = this.instanceAllocationService.currentInstanceResource;
  currentInstanceResourceError = computed(() => convertError(this.currentInstanceResource.error()));

  onAllocate() {
    this.dialog.open(InstanceNotAllocatedDialog, {
      disableClose: true,
      data: { projectId: this.projectId() },
    });
  }
}
