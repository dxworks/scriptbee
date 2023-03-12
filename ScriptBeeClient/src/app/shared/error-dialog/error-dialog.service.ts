import { Injectable } from '@angular/core';
import { ErrorDialogComponent } from './error-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root',
})
export class ErrorDialogService {
  constructor(private dialog: MatDialog) {}

  displayDialogErrorMessage(mainProblem: string, errorMessage: string) {
    if (errorMessage == null || errorMessage === '') {
      this.dialog.open(ErrorDialogComponent, { data: { mainProblem: mainProblem, errorMessage: '' } });
    } else {
      this.dialog.open(ErrorDialogComponent, { data: { mainProblem: mainProblem, errorMessage: errorMessage } });
    }
  }
}
