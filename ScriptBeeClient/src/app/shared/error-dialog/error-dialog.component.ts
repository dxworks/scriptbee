import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ErrorDialogData } from './error-dialog-data';

@Component({
  selector: 'app-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.scss']
})
export class ErrorDialogComponent {

  constructor(public dialogRef: MatDialogRef<ErrorDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: ErrorDialogData) {
    if (!data) {
      // action
    } else if (!data.mainProblem) {
      this.data.mainProblem = '';
    } else if (!data.errorMessage) {
      this.data.errorMessage = '';
    }
  }
}
