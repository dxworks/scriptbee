import { Component, inject, model } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatInput } from '@angular/material/input';

export interface InstanceNotAllocatedDialogData {
  projectId: string;
  scriptId: string;
  currentScriptName: string;
}

@Component({
  selector: 'app-rename-file-dialog',
  templateUrl: './rename-file-dialog.component.html',
  styleUrls: ['./rename-file-dialog.component.scss'],
  imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatExpansionModule, MatSelectModule, MatButtonModule, FormsModule, MatInput, MatDialogClose],
})
export class RenameFileDialog {
  public data = inject<InstanceNotAllocatedDialogData>(MAT_DIALOG_DATA);
  public dialogRef = inject(MatDialogRef<RenameFileDialog>);

  readonly newName = model(this.data.currentScriptName);

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
