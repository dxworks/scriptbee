import { Component, Inject } from '@angular/core';
import { ScriptsStore } from '../../../stores/scripts-store.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { EditParametersDialogData } from './edit-parameters-dialog-data';
import { Parameter } from '../../../services/script-types';
import { ApiErrorMessage } from '../../../../shared/api-error-message';

@Component({
  selector: 'app-edit-parameters-dialog',
  templateUrl: './edit-parameters-dialog.component.html',
  styleUrls: ['./edit-parameters-dialog.component.scss'],
  providers: [ScriptsStore],
})
export class EditParametersDialogComponent {
  updateScriptError: ApiErrorMessage | undefined = undefined;
  isUpdatingScript = false;
  constructor(
    public dialogRef: MatDialogRef<EditParametersDialogData>,
    @Inject(MAT_DIALOG_DATA) public data: EditParametersDialogData,
    private store: ScriptsStore
  ) {
    this.store.updateScriptResult.subscribe((result) => {
      this.isUpdatingScript = false;
      if (result) {
        this.dialogRef.close();
      }
    });

    this.store.updateScriptError.subscribe((error) => {
      this.isUpdatingScript = false;
      this.updateScriptError = error;
    });
  }

  onParametersChange(parameters: Parameter[]) {
    this.data.parameters = parameters;
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  onUpdateClick() {
    this.isUpdatingScript = true;
    this.updateScriptError = undefined;
    this.store.updateScript({
      id: this.data.scriptId,
      projectId: this.data.projectId,
      parameters: this.data.parameters.map((parameter) => ({
        name: parameter.name,
        type: parameter.type,
        value: parameter.value,
      })),
    });
  }
}
