import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {CreateProjectDialogData} from './create-project-dialog-data';

@Component({
  selector: 'app-create-project-dialog',
  templateUrl: './create-project-dialog.component.html',
  styleUrls: ['./create-project-dialog.component.scss']
})
export class CreateProjectDialogComponent {

  constructor(public dialogRef: MatDialogRef<CreateProjectDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public data: CreateProjectDialogData) {
    if (!data || !data.projectName) {
      this.data = {projectName: ''};
    }
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }
}
