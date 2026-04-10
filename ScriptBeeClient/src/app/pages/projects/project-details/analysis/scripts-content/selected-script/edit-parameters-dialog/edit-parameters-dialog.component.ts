import { Component, computed, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { ScriptParameter } from '../../../../../../../types/script-types';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { ScriptParametersListComponent } from '../../../../../../../components/script-parameters-list/script-parameters-list.component';
import { ProjectStructureService } from '../../../../../../../services/projects/project-structure.service';
import { LoadingProgressBarComponent } from '../../../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { finalize } from 'rxjs';

export interface EditParametersDialogData {
  projectId: string;
  scriptId: string;
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
    LoadingProgressBarComponent,
  ],
})
export class EditParametersDialogComponent {
  parameters = signal<ScriptParameter[]>([]);
  hasParameterErrors = signal<boolean>(true);
  isUpdateLoading = signal(false);

  isUpdateDisabled = computed(() => this.hasParameterErrors() || this.isUpdateLoading());

  public dialogRef = inject(MatDialogRef<EditParametersDialogData>);
  public data = inject<EditParametersDialogData>(MAT_DIALOG_DATA);
  private projectStructureService = inject(ProjectStructureService);

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
    this.isUpdateLoading.set(true);
    this.projectStructureService
      .updateProjectScript(this.data.projectId, this.data.scriptId, undefined, this.parameters())
      .pipe(finalize(() => this.isUpdateLoading.set(false)))
      .subscribe({ next: () => this.dialogRef.close() });
  }
}
