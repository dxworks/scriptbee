import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {CreateScriptDialogData} from './create-script-dialog-data';
import {CreateScriptStore} from '../../../stores/create-script.store';
import {map} from 'rxjs/operators';
import {Parameter} from '../../../services/script-types';
import {ApiErrorMessage} from '../../../../shared/api-error-message';

@Component({
  selector: 'app-create-script-dialog',
  templateUrl: './create-script-dialog.component.html',
  styleUrls: ['./create-script-dialog.component.scss'],
  providers: [CreateScriptStore],
})
export class CreateScriptDialogComponent {
  availableScriptLanguages$ = this.store.availableLanguages.pipe(map((languages) => languages.map((language) => language.name)));

  createScriptError: ApiErrorMessage | undefined = undefined;
  isCreatingScript = false;

  scriptPath = '';
  scriptLanguage = '';

  parameters: Parameter[] = [];

  constructor(
    public dialogRef: MatDialogRef<CreateScriptDialogComponent>,
    private store: CreateScriptStore,
    @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData
  ) {
    this.store.loadAvailableLanguages();
    this.store.createScriptResult.subscribe((result) => {
      this.isCreatingScript = false;
      if (result) {
        this.dialogRef.close();
      }
    });
    this.store.createScriptError.subscribe((error) => {
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
    this.store.createScript({
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
