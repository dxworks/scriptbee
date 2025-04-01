import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { CreateScriptDialogData } from '../../../pages/projects/project-details/analysis/script-tree/create-script-dialog/create-script-dialog.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-instance-not-allocated-dialog',
  templateUrl: './instance-not-allocated-dialog.component.html',
  styleUrls: ['./instance-not-allocated-dialog.component.scss'],
  imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatExpansionModule, MatSelectModule, MatButtonModule, FormsModule],
})
export class InstanceNotAllocatedDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData,
    public dialogRef: MatDialogRef<InstanceNotAllocatedDialog>
  ) {}

  onCloseClick(): void {
    this.dialogRef.close();
  }

  onAllocateClick(): void {}
}
