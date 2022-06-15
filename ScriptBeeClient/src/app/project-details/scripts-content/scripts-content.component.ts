import {Component, OnInit} from '@angular/core';
import {FileSystemService} from '../../services/file-system/file-system.service';
import {ProjectDetailsService} from '../project-details.service';
import {FileTreeNode} from './fileTreeNode';
import {ActivatedRoute, Router} from '@angular/router';
import {CreateScriptDialogComponent} from './create-script-dialog/create-script-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {MatSnackBar} from '@angular/material/snack-bar';
import {OutputFilesService} from '../../services/output/output-files.service';
import {OutputFile} from '../../services/output/output-file';
import {TreeNode} from '../../shared/tree-node';

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss']
})
export class ScriptsContentComponent implements OnInit {

  fileStructureTree: FileTreeNode[] = [];
  consoleOutput = '';
  outputErrors = '';
  outputFiles: OutputFile[] = [];
  projectId = '';
  runId = '';

  constructor(private fileSystemService: FileSystemService, private projectDetailsService: ProjectDetailsService,
              private router: Router, private route: ActivatedRoute, private dialog: MatDialog, private snackBar: MatSnackBar,
              private outputFilesService: OutputFilesService) {
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
      this.outputErrors = message;
      this.consoleOutput = '';
      this.outputFiles = [];

      if (!message) {
        this.projectDetailsService.lastRunResult.subscribe(runResult => {
          if (runResult == null) {
            return;
          }
          this.projectId = runResult.projectId;
          this.runId = runResult.runId;
          this.outputFilesService.getConsoleOutputContent(runResult.consoleOutputName).subscribe(consoleContent => {
            this.consoleOutput = consoleContent;
          });

          this.outputFiles = runResult.outputFiles;
        });
      }
    });
  }

  private loadProjectFileStructure(callback?: () => void) {
    this.projectDetailsService.project.subscribe(project => {
      if (project) {
        this.fileSystemService.getFileSystem(project.projectId).subscribe(fileTreeNode => {
          if (fileTreeNode) {
            this.fileStructureTree = [fileTreeNode];
            if (callback) {
              callback();
            }
          }
        });
      }
    });
  }
}
