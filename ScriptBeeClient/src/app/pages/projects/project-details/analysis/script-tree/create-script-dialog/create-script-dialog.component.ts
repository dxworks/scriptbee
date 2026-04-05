import { Component, computed, inject, signal } from '@angular/core';
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
import { ProjectStructureService } from '../../../../../../services/projects/project-structure.service';
import { LoadingProgressBarComponent } from '../../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { finalize } from 'rxjs';

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
    LoadingProgressBarComponent,
  ],
})
export class CreateScriptDialogComponent {
  scriptPath = signal('');
  scriptLanguage = signal('');
  parameters = signal<ScriptParameter[]>([]);
  hasParameterErrors = signal<boolean>(true);
  isCreateLoading = signal(false);

  isOkDisabled = computed(() => !this.scriptPath() || !this.scriptLanguage() || this.hasParameterErrors() || this.isCreateLoading());

  public data = inject<CreateScriptDialogData>(MAT_DIALOG_DATA);
  public dialogRef = inject(MatDialogRef<CreateScriptDialogComponent>);
  private projectStructureService = inject(ProjectStructureService);

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
    this.isCreateLoading.set(true);
    this.projectStructureService
      .createProjectScript(this.data.projectId, this.scriptPath(), this.scriptLanguage(), this.parameters())
      .pipe(finalize(() => this.isCreateLoading.set(false)))
      .subscribe({ next: () => this.dialogRef.close() });
  }
}
