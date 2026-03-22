import { Component, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { InstanceService } from '../../../services/instances/instance.service';
import { LoadingProgressBarComponent } from '../../loading-progress-bar/loading-progress-bar.component';
import { finalize } from 'rxjs';

export interface InstanceNotAllocatedDialogData {
  projectId: string;
}

@Component({
  selector: 'app-instance-not-allocated-dialog',
  templateUrl: './instance-not-allocated-dialog.component.html',
  styleUrls: ['./instance-not-allocated-dialog.component.scss'],
  imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatExpansionModule, MatSelectModule, MatButtonModule, FormsModule, LoadingProgressBarComponent],
})
export class InstanceNotAllocatedDialog {
  isAllocateLoading = signal(false);

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
    this.isAllocateLoading.set(true);
    this.instanceService
      .allocateInstance(this.data.projectId)
      .pipe(finalize(() => this.isAllocateLoading.set(false)))
      .subscribe({ next: () => this.dialogRef.close() });
  }
}
