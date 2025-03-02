import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { EditParametersDialogData } from './edit-parameters-dialog-data';
import { Parameter } from '../../../services/script-types';
import { UpdateScriptStore } from '../../../stores/update-script-store.service';

@Component({
  selector: 'app-edit-parameters-dialog',
  templateUrl: './edit-parameters-dialog.component.html',
  styleUrls: ['./edit-parameters-dialog.component.scss'],
  providers: [UpdateScriptStore],
})
export class EditParametersDialogComponent {
  isUpdatingScript$ = this.updateScriptStore.updateScriptLoading;
  updateScriptError$ = this.updateScriptStore.updateScriptError;

  constructor(
    public dialogRef: MatDialogRef<EditParametersDialogData>,
    @Inject(MAT_DIALOG_DATA) public data: EditParametersDialogData,
    private updateScriptStore: UpdateScriptStore
  ) {
    this.updateScriptStore.updateScriptResult.subscribe((result) => {
      if (result) {
        this.dialogRef.close();
      }
    });
  }

  onParametersChange(parameters: Parameter[]) {
    this.data.parameters = parameters;
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  isUpdateDisabled(): boolean {
    const parametersAreValid = this.data.parameters.every((parameter) => !parameter.nameError);

    return !parametersAreValid;
  }

  onUpdateClick() {
    this.updateScriptStore.updateScript({
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
