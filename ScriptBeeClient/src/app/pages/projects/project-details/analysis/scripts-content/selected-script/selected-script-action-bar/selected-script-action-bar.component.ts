import { Component, computed, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltip } from '@angular/material/tooltip';
import { CdkCopyToClipboard } from '@angular/cdk/clipboard';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { SafeUrlPipe } from '../../../../../../../pipes/safe-url.pipe';
import { ProjectScript } from '../../../../../../../types/project';
import { EditParametersDialogComponent } from '../edit-parameters-dialog/edit-parameters-dialog.component';
import { AnalysisService } from '../../../../../../../services/analysis/analysis.service';
import { UserFolderPathService } from '../../../../../../../services/common/user-folder-path.service';

@Component({
  selector: 'app-selected-script-action-bar',
  templateUrl: './selected-script-action-bar.component.html',
  styleUrls: ['./selected-script-action-bar.component.scss'],
  imports: [MatButtonModule, MatIconModule, MatTooltip, CdkCopyToClipboard, SafeUrlPipe, FormsModule, SafeUrlPipe],
})
export class SelectedScriptActionBarComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();
  script = input.required<ProjectScript>();
  saveStatus = input<string | undefined>();

  isLoadingResults = signal<boolean>(false);
  statusUrlChange = output<string | undefined>();

  absoluteSrcPath = computed(() => {
    const userFolderPath = UserFolderPathService.getUserFolderPath(this.projectId()).replaceAll('\\', '/');

    return `${userFolderPath}/projects/${this.projectId()}/src`;
  });

  absoluteFilePath = computed(() => {
    return `${this.absoluteSrcPath()}/${this.script().path}`;
  });

  private analysisService = inject(AnalysisService);
  private snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);

  onRunScriptButtonClick() {
    this.isLoadingResults.set(true);
    this.statusUrlChange.emit(undefined);

    this.analysisService.triggerAnalysis(this.projectId(), this.instanceId(), this.script().id).subscribe({
      next: (statusUrl) => {
        this.isLoadingResults.set(false);
        this.statusUrlChange.emit(statusUrl);
      },
      error: () => {
        this.isLoadingResults.set(false);

        this.snackBar.open('Could not run script!', 'Ok', {
          duration: 4000,
        });
      },
    });
  }

  onCopyToClipboardButtonClick() {
    this.snackBar.open('Path copied successfully!', 'Ok', {
      duration: 2000,
    });
  }

  onEditParametersButtonClick() {
    this.dialog.open(EditParametersDialogComponent, {
      disableClose: true,
      data: {
        projectId: this.projectId(),
        scriptId: this.script().id,
        parameters: this.script().parameters,
      },
    });
  }
}
