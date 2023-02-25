import { Component, OnDestroy, OnInit } from '@angular/core';
import { FileSystemService } from '../../../../services/file-system/file-system.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateScriptDialogComponent } from '../create-script-dialog/create-script-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OutputFile } from '../../../../services/output/output-file';
import { filter } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { selectProjectDetails } from '../../../../state/project-details/project-details.selectors';
import { fetchScriptTree } from '../../../../state/script-tree/script-tree.actions';
import { selectScriptTreeLeafClick } from '../../../../state/script-tree/script-tree.selectors';
import { selectLastRunOutput } from '../../../../state/outputs/output.selectors';

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
    private fileSystemService: FileSystemService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private store: Store
  ) {}

  ngOnInit(): void {
    this.store
      .select(selectProjectDetails)
      .pipe(filter((x) => !!x))
      .subscribe({
        next: (projectDetails) => {
          this.projectId = projectDetails.data.projectId;
          this.store.dispatch(fetchScriptTree({ projectId: projectDetails.data.projectId }));

          this.fileSystemService.addFileWatcher(this.projectId).subscribe();
        },
      });

    this.store
      .select(selectScriptTreeLeafClick)
      .pipe(filter((x) => !!x))
      .subscribe({
        next: (node) => {
          void this.router.navigate([node.id], { relativeTo: this.route });
        },
      });

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
