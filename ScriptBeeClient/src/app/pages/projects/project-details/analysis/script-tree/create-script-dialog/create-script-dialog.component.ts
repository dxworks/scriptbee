import { Component, computed, Inject, signal } from '@angular/core';
import { ScriptParameter } from '../../../../../../types/script-types';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormField } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { ScriptParametersListComponent } from '../../../../../../components/script-parameters-list/script-parameters-list.component';
import { CreateScriptDialogScriptLanguageComponent } from './create-script-dialog-script-language/create-script-dialog-script-language.component';
import { apiHandler } from '../../../../../../utils/apiHandler';
import { ProjectStructureService } from '../../../../../../services/projects/project-structure.service';
import { ErrorStateComponent } from '../../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../../components/loading-progress-bar/loading-progress-bar.component';

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
    CreateScriptDialogScriptLanguageComponent,
    ErrorStateComponent,
    LoadingProgressBarComponent,
  ],
})
export class CreateScriptDialogComponent {
  scriptPath = signal('');
  scriptLanguage = signal('');
  parameters = signal<ScriptParameter[]>([]);
  hasParameterErrors = signal<boolean>(true);

  isOkDisabled = computed(() => !this.scriptPath() || !this.scriptLanguage() || this.hasParameterErrors() || this.createScriptHandler.isLoading());

  createScriptHandler = apiHandler(
    (params: { projectId: string; scriptPath: string; scriptLanguage: string; parameters: ScriptParameter[] }) =>
      this.projectStructureService.createProjectScript(params.projectId, params.scriptPath, params.scriptLanguage, params.parameters),
    () => {
      this.dialogRef.close();
    }
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData,
    public dialogRef: MatDialogRef<CreateScriptDialogComponent>,
    private projectStructureService: ProjectStructureService
  ) {}

  onParametersChange(parameters: ScriptParameter[]) {
    this.parameters.set(parameters);
  }

  onHasParameterErrors(hasErrors: boolean) {
    this.hasParameterErrors.set(hasErrors);
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    this.createScriptHandler.execute({
      projectId: this.data.projectId,
      scriptPath: this.scriptPath(),
      scriptLanguage: this.scriptLanguage(),
      parameters: this.parameters(),
    });
  }
}
