import { Component, computed, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { ScriptParameter } from '../../../../../../../types/script-types';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { ScriptParametersListComponent } from '../../../../../../../components/script-parameters-list/script-parameters-list.component';
import { ProjectStructureService } from '../../../../../../../services/projects/project-structure.service';
import { apiHandler } from '../../../../../../../utils/apiHandler';
import { ErrorStateComponent } from '../../../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../../../components/loading-progress-bar/loading-progress-bar.component';

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
    ErrorStateComponent,
    LoadingProgressBarComponent,
  ],
})
export class EditParametersDialogComponent {
  parameters = signal<ScriptParameter[]>([]);
  hasParameterErrors = signal<boolean>(true);

  isUpdateDisabled = computed(() => this.hasParameterErrors() || this.updateScriptHandler.isLoading());

  updateScriptHandler = apiHandler(
    (params: { projectId: string; parameters: ScriptParameter[] }) =>
      this.projectStructureService.updateProjectScript(params.projectId, undefined, params.parameters),
    () => {
      this.dialogRef.close();
    }
  );

  constructor(
    public dialogRef: MatDialogRef<EditParametersDialogData>,
    @Inject(MAT_DIALOG_DATA) public data: EditParametersDialogData,
    private projectStructureService: ProjectStructureService
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
    this.updateScriptHandler.execute({
      projectId: this.data.projectId,
      parameters: this.parameters(),
    });
  }
}
