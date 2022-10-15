import { Component, OnInit } from '@angular/core';
import { FileSystemService } from '../../services/file-system/file-system.service';
import { ProjectDetailsService } from '../project-details.service';
import { FileTreeNode } from './fileTreeNode';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateScriptDialogComponent } from './create-script-dialog/create-script-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OutputFilesService } from '../../services/output/output-files.service';
import { OutputFile } from '../../services/output/output-file';
import { TreeNode } from '../../shared/tree-node';
import { filter } from 'rxjs/operators';
import { RunScriptResult } from '../../services/run-script/run-script-result';
import { ResultsService } from "../../services/plugin/results.service";

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss']
})
export class ScriptsContentComponent implements OnInit {

  fileStructureTree: FileTreeNode[] = [];
  outputFiles: OutputFile[] = [];
  projectId = '';
  runId = '';

  constructor(private fileSystemService: FileSystemService, private projectDetailsService: ProjectDetailsService,
              private router: Router, private route: ActivatedRoute, private dialog: MatDialog, private snackBar: MatSnackBar,
              private outputFilesService: OutputFilesService, public resultsService: ResultsService) {
  }

  ngOnInit(): void {
    this.loadProjectFileStructure();
    this.getOutput();
  }

  onLeafClick(node: TreeNode) {
    const fileNode = node as FileTreeNode;
    this.router.navigate([fileNode.srcPath], {relativeTo: this.route});
  }

  onCreateNewScriptButtonClick() {
    const dialogRef = this.dialog.open(CreateScriptDialogComponent, {
      width: '300px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(scriptPath => {
      if (scriptPath) {
        this.loadProjectFileStructure(() => {
          const newScriptPath = scriptPath.replaceAll('/', '\\');
          this.router.navigate([newScriptPath], {relativeTo: this.route});
        });
      }
    }, (error: any) => {
      this.snackBar.open('Could not create script!', 'Ok', {
        duration: 4000
      });
    });
  }

  private getOutput() {
    this.projectDetailsService.lastRunErrorMessage.subscribe(message => {
      this.outputFiles = [];

      if (!message) {
        this.projectDetailsService.lastRunResult.pipe(filter(x => x !== undefined)).subscribe(runResult => {

          if (runResult == null) {
            return;
          }
          this.projectId = runResult.projectId;
          this.runId = runResult.runId;
        });
      }
    });
  }

  private loadProjectFileStructure(callback?: () => void) {
    // todo
    // this.projectDetailsService.project.subscribe(project => {
    //   if (project) {
    //     this.fileSystemService.getFileSystem(project.projectId).subscribe(fileTreeNode => {
    //       if (fileTreeNode) {
    //         this.fileStructureTree = [fileTreeNode];
    //         if (callback) {
    //           callback();
    //         }
    //       }
    //     });
    //   }
    // });
  }

  private areRunResultsEqual(runRes1: RunScriptResult, runRes2: RunScriptResult) {
    return runRes1 && runRes2 && runRes1.runId == runRes2.runId && runRes1.projectId == runRes2.projectId;
  }
}
