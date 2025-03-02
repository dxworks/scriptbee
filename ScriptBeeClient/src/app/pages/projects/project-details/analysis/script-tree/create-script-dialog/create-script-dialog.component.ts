import { Component, computed, Inject, signal } from '@angular/core';
import { Parameter, ScriptLanguage } from '../../../../../../types/script-types';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormField } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { ScriptParametersListComponent } from '../../../../../../components/script-parameters-list/script-parameters-list.component';

export interface CreateScriptDialogData {
  projectId: string;
}

@Component({
  selector: 'app-create-script-dialog',
  templateUrl: './create-script-dialog.component.html',
  styleUrls: ['./create-script-dialog.component.scss'],
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatExpansionModule,
    MatFormField,
    MatSelectModule,
    MatButtonModule,
    FormsModule,
    MatInput,
    ScriptParametersListComponent,
  ],
})
export class CreateScriptDialogComponent {
  // TODO FIXIT: load script languages from API
  availableScriptLanguages = signal<ScriptLanguage[]>([{ name: 'C#', extension: '.cs' }]);

  scriptPath = signal('');
  scriptLanguage = signal('');
  parameters = signal<Parameter[]>([]);
  hasParameterErrors = signal<boolean>(true);

  isOkDisabled = computed(() => !this.scriptPath() || !this.scriptLanguage() || this.hasParameterErrors());

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData,
    public dialogRef: MatDialogRef<CreateScriptDialogComponent>
  ) {}

  onParametersChange(parameters: Parameter[]) {
    this.parameters.set(parameters);
  }

  onHasParameterErrors(hasErrors: boolean) {
    this.hasParameterErrors.set(hasErrors);
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    // TODO FIXIT: call api
    // this.createScriptStore.createScript({
    //   projectId: this.data.projectId,
    //   filePath: this.scriptPath,
    //   scriptLanguage: this.scriptLanguage,
    //   parameters: this.parameters.map((parameter) => ({
    //     name: parameter.name,
    //     type: parameter.type,
    //     value: parameter.value,
    //   })),
    // });
  }
}
