import { Component, OnDestroy, OnInit } from '@angular/core';
import { AngularSplitModule } from 'angular-split';
import { ScriptsContentComponent } from './scripts-content/scripts-content.component';
import { ScriptTreeComponent } from './script-tree/script-tree.component';

@Component({
  selector: 'app-analysis',
  templateUrl: './analysis.component.html',
  styleUrls: ['./analysis.component.scss'],
  imports: [AngularSplitModule, ScriptsContentComponent, ScriptTreeComponent],
})
export class AnalysisComponent implements OnInit, OnDestroy {
  // projectId = '';
  //
  // outputFiles: OutputFile[] = [];
  // runIndex: number | undefined;
  // outputErrorsId: string | undefined;
  // consoleOutputId: string | undefined;

  constructor() {} // private store: Store // private snackBar: MatSnackBar, // private dialog: MatDialog, // private route: ActivatedRoute, // private router: Router, // private fileSystemService: FileSystemService, // private projectStore: ProjectStore,

  ngOnInit(): void {
    // this.projectId = this.projectStore.getProjectId();
    // this.fileSystemService.addFileWatcher(this.projectId).subscribe();
    //
    // this.store.select(selectLastRunOutput).subscribe({
    //   next: (outputState) => {
    //     this.outputErrorsId = outputState.results.filter((x) => x.type === 'RunError').map((x) => x.id)[0];
    //     this.consoleOutputId = outputState.results.filter((x) => x.type === 'Console').map((x) => x.id)[0];
    //     this.outputFiles = outputState.results
    //       .filter((x) => x.type === 'File')
    //       .map((x) => ({
    //         fileId: x.id,
    //         fileName: x.name,
    //       }));
    //     this.runIndex = outputState.runIndex;
    //   },
    // });
  }

  ngOnDestroy(): void {
    // this.fileSystemService.removeFileWatcher(this.projectId).subscribe();
  }
}
