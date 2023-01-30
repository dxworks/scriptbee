import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-project-dialog',
  templateUrl: './delete-project-dialog.component.html',
  styleUrls: ['./delete-project-dialog.component.scss'],
})
export class DeleteProjectDialogComponent {
  constructor(public dialogRef: MatDialogRef<DeleteProjectDialogComponent>) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }
}
