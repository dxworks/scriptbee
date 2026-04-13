import { Component, computed, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { convertError } from '../../../../utils/api';
import { MatIconModule } from '@angular/material/icon';
import { InstanceNotAllocatedDialog } from '../../../../components/dialogs/instance-not-allocated-dialog/instance-not-allocated-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { InstanceAllocationService } from '../../../../services/instances/instance-allocation.service';
import { MatButton } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { ProjectStateService } from '../../../../services/projects/project-state.service';

@Component({
  selector: 'app-project-model-page',
  templateUrl: './project-model-page.component.html',
  styleUrls: ['./project-model-page.component.scss'],
  imports: [MatCardModule, ErrorStateComponent, LoadingProgressBarComponent, MatIconModule, MatButton, RouterOutlet],
})
export class ProjectModelPage {
  private instanceAllocationService = inject(InstanceAllocationService);
  private dialog = inject(MatDialog);

  currentInstanceResource = this.instanceAllocationService.currentInstanceResource;
  currentInstanceResourceError = computed(() => convertError(this.currentInstanceResource.error()));

  private projectStateService = inject(ProjectStateService);

  onAllocate() {
    this.dialog.open(InstanceNotAllocatedDialog, {
      disableClose: true,
      data: { projectId: this.projectStateService.currentProjectId() },
    });
  }
}
