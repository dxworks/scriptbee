import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CreateScriptDialogData } from './create-script-dialog-data';
import { map } from 'rxjs/operators';
import { Parameter } from '../../../services/script-types';
import { ApiErrorMessage } from '../../../../shared/api-error-message';
import { CreateScriptStore } from '../../../stores/create-script-store.service';
import { ScriptsStore } from '../../../stores/scripts-store.service';

@Component({
  selector: 'app-create-script-dialog',
  templateUrl: './create-script-dialog.component.html',
  styleUrls: ['./create-script-dialog.component.scss'],
  providers: [CreateScriptStore],
})
export class CreateScriptDialogComponent {
  availableScriptLanguages$ = this.scriptsStore.availableLanguages.pipe(map((languages) => languages.map((language) => language.name)));

  createScriptError: ApiErrorMessage | undefined = undefined;
  isCreatingScript = false;

  scriptPath = '';
  scriptLanguage = '';

  parameters: Parameter[] = [];

  constructor(
    public dialogRef: MatDialogRef<CreateScriptDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData,
    private createScriptStore: CreateScriptStore,
    private scriptsStore: ScriptsStore
  ) {
    this.scriptsStore.loadAvailableLanguages();
    this.createScriptStore.createScriptResult.subscribe((result) => {
      this.isCreatingScript = false;
      if (result) {
        this.dialogRef.close();
      }
    });
    this.createScriptStore.createScriptError.subscribe((error) => {
      this.isCreatingScript = false;
      this.createScriptError = error;
    });
  }

  onParametersChange(parameters: Parameter[]) {
    this.parameters = parameters;
  }

  isOkDisabled(): boolean {
    const parametersAreValid = this.parameters.every((parameter) => !parameter.nameError);

    return !this.scriptPath || !this.scriptLanguage || !parametersAreValid;
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    this.isCreatingScript = true;
    this.createScriptError = undefined;
    this.createScriptStore.createScript({
      projectId: this.data.projectId,
      filePath: this.scriptPath,
      scriptLanguage: this.scriptLanguage,
      parameters: this.parameters.map((parameter) => ({
        name: parameter.name,
        type: parameter.type,
        value: parameter.value,
      })),
    });
  }
}
