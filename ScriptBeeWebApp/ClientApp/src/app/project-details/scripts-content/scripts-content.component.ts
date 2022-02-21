import {Component, OnInit} from '@angular/core';
import {FileSystemService} from '../../services/file-system/file-system.service';
import {ProjectDetailsService} from '../project-details.service';
import {FileTreeNode} from './fileTreeNode';
import {ActivatedRoute, Router} from '@angular/router';
import {CreateScriptDialogComponent} from './create-script-dialog/create-script-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss']
})
export class ScriptsContentComponent implements OnInit {

  fileStructureTree: FileTreeNode[] = [];

  constructor(private fileSystemService: FileSystemService, private projectDetailsService: ProjectDetailsService,
              private router: Router, private route: ActivatedRoute, private dialog: MatDialog, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.loadProjectFileStructure();
  }

  onLeafClick(node: FileTreeNode) {
    this.router.navigate([node.srcPath], {relativeTo: this.route});
  }

  onCreateNewScriptButtonClick() {
    const dialogRef = this.dialog.open(CreateScriptDialogComponent, {
      width: '300px',
    });

    dialogRef.afterClosed().subscribe(scriptPath => {
      if (scriptPath) {
        this.loadProjectFileStructure(() => {
          this.router.navigate([scriptPath], {relativeTo: this.route});
        });
      }
    }, (error: any) => {
      this.snackBar.open('Could not create script!', 'Ok', {
        duration: 4000
      });
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
