import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { LoadingProgressBarComponent } from '../../loading-progress-bar/loading-progress-bar.component';
import { finalize } from 'rxjs';
import { InstanceAllocationService } from '../../../services/instances/instance-allocation.service';

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

  public data = inject<InstanceNotAllocatedDialogData>(MAT_DIALOG_DATA);
  public dialogRef = inject(MatDialogRef<InstanceNotAllocatedDialog>);
  private instanceAllocationService = inject(InstanceAllocationService);

  onCloseClick(): void {
    this.dialogRef.close();
  }

  onAllocateClick(): void {
    this.isAllocateLoading.set(true);
    this.instanceAllocationService
      .allocateInstance(this.data.projectId)
      .pipe(finalize(() => this.isAllocateLoading.set(false)))
      .subscribe({
        next: () => {
          this.dialogRef.close();
        },
      });
  }
}
