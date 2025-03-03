import { Component, input } from '@angular/core';
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

@Component({
  selector: 'app-selected-script-action-bar',
  templateUrl: './selected-script-action-bar.component.html',
  styleUrls: ['./selected-script-action-bar.component.scss'],
  imports: [MatButtonModule, MatIconModule, MatTooltip, CdkCopyToClipboard, SafeUrlPipe, FormsModule, SafeUrlPipe],
})
export class SelectedScriptActionBarComponent {
  projectId = input.required<string>();
  script = input.required<ProjectScript>();

  constructor(
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  onRunScriptButtonClick() {
    //   this.isLoadingResults = true;
    //   this.runScriptService.runScriptFromPath(this.projectId, this.scriptPath, this.getLanguage(this.scriptPath)).subscribe({
    //     next: (run) => {
    //       this.isLoadingResults = false;
    //       // todo: remove this global store and use a component store
    //
    //       if (run.results.filter((r) => r.type === 'RunError').length > 0) {
    //         this.snackBar.open('Script run has errors!', 'Ok', {
    //           duration: 4000,
    //         });
    //       }
    //
    //       this.store.dispatch(
    //         setOutput({
    //           runIndex: run.index,
    //           scriptName: run.scriptName,
    //           results: run.results,
    //         })
    //       );
    //     },
    //     error: (err) => {
    //       this.isLoadingResults = false;
    //
    //       if (err.status === 404 && err.error.includes('Could not find project')) {
    //         this.snackBar.open('Project Context might not be loaded!', 'Ok', {
    //           duration: 4000,
    //         });
    //         return;
    //       }
    //
    //       this.snackBar.open('Could not run script!', 'Ok', {
    //         duration: 4000,
    //       });
    //     },
    //   });
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
        parameters: this.script().parameters,
      },
    });
  }
}
