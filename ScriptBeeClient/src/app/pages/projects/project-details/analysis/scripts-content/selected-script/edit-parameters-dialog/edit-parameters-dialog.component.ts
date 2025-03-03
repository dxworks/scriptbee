import { Component, computed, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { ScriptParameter } from '../../../../../../../types/script-types';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { ScriptParametersListComponent } from '../../../../../../../components/script-parameters-list/script-parameters-list.component';

export interface EditParametersDialogData {
  projectId: string;
  parameters: ScriptParameter[];
}

@Component({
  selector: 'app-edit-parameters-dialog',
  templateUrl: './edit-parameters-dialog.component.html',
  styleUrls: ['./edit-parameters-dialog.component.scss'],
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatExpansionModule,
    MatSelectModule,
    MatButtonModule,
    FormsModule,
    ScriptParametersListComponent,
  ],
})
export class EditParametersDialogComponent {
  parameters = signal<ScriptParameter[]>([]);
  hasParameterErrors = signal<boolean>(true);

  isUpdateDisabled = computed(() => this.hasParameterErrors());

  constructor(
    public dialogRef: MatDialogRef<EditParametersDialogData>,
    @Inject(MAT_DIALOG_DATA) public data: EditParametersDialogData
  ) {}

  onParametersChange(parameters: ScriptParameter[]) {
    this.parameters.set(parameters);
  }

  onHasParameterErrors(hasErrors: boolean) {
    this.hasParameterErrors.set(hasErrors);
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  onUpdateClick() {
    // TODO FIXIT: call api
    // this.updateScriptStore.updateScript({
    //   id: this.data.scriptId,
    //   projectId: this.data.projectId,
    //   parameters: this.data.parameters.map((parameter) => ({
    //     name: parameter.name,
    //     type: parameter.type,
    //     value: parameter.value,
    //   })),
    // });
  }
}
