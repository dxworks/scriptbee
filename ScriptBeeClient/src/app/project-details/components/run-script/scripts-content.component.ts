import { Component, OnDestroy, OnInit } from '@angular/core';
import { FileSystemService } from '../../../services/file-system/file-system.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateScriptDialogComponent } from './create-script-dialog/create-script-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OutputFile } from '../../../services/output/output-file';
import { Store } from '@ngrx/store';
import { selectLastRunOutput } from '../../../state/outputs/output.selectors';
import { ProjectStore } from '../../stores/project-store.service';

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss'],
})
export class ScriptsContentComponent implements OnInit, OnDestroy {
  projectId = '';

  outputFiles: OutputFile[] = [];
  runIndex: number | undefined;
  outputErrorsId: string | undefined;
  consoleOutputId: string | undefined;

  constructor(
    private projectStore: ProjectStore,
    private fileSystemService: FileSystemService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private store: Store
  ) {}

  ngOnInit(): void {
    this.projectId = this.projectStore.getProjectId();
    this.fileSystemService.addFileWatcher(this.projectId).subscribe();

    this.store.select(selectLastRunOutput).subscribe({
      next: (outputState) => {
        this.outputErrorsId = outputState.results.filter((x) => x.type === 'RunError').map((x) => x.id)[0];
        this.consoleOutputId = outputState.results.filter((x) => x.type === 'Console').map((x) => x.id)[0];
        this.outputFiles = outputState.results
          .filter((x) => x.type === 'File')
          .map((x) => ({
            fileId: x.id,
            fileName: x.name,
          }));
        this.runIndex = outputState.runIndex;
      },
    });
  }

  ngOnDestroy(): void {
    this.fileSystemService.removeFileWatcher(this.projectId).subscribe();
  }

  onCreateNewScriptButtonClick() {
    this.dialog.open(CreateScriptDialogComponent, {
      disableClose: true,
      data: { projectId: this.projectId },
    });
  }
}
