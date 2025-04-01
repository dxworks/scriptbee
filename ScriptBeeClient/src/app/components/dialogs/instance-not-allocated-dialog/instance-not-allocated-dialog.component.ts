import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { InstanceService } from '../../../services/instances/instance.service';
import { ErrorStateComponent } from '../../error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../loading-progress-bar/loading-progress-bar.component';
import { apiHandler } from '../../../utils/apiHandler';

export interface InstanceNotAllocatedDialogData {
  projectId: string;
}

@Component({
  selector: 'app-instance-not-allocated-dialog',
  templateUrl: './instance-not-allocated-dialog.component.html',
  styleUrls: ['./instance-not-allocated-dialog.component.scss'],
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatExpansionModule,
    MatSelectModule,
    MatButtonModule,
    FormsModule,
    ErrorStateComponent,
    LoadingProgressBarComponent,
  ],
})
export class InstanceNotAllocatedDialog {
  allocateInstanceHandler = apiHandler(
    (params: { projectId: string }) => this.instanceService.allocateInstance(params.projectId),
    () => {
      this.dialogRef.close();
    }
  );

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: InstanceNotAllocatedDialogData,
    public dialogRef: MatDialogRef<InstanceNotAllocatedDialog>,
    private instanceService: InstanceService
  ) {}

  onCloseClick(): void {
    this.dialogRef.close();
  }

  onAllocateClick(): void {
    this.allocateInstanceHandler.execute({
      projectId: this.data.projectId,
    });
  }
}
