import {Component} from '@angular/core';
import {ProjectService} from '../../services/project/project.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ProjectDetailsService} from '../project-details.service';
import {TreeNode} from '../../shared/tree-node';
import {UploadService} from '../../services/upload/upload.service';
import {LoaderService} from '../../services/loader/loader.service';
import {MatDialog} from '@angular/material/dialog';
import {ErrorDialogComponent} from './error-dialog/error-dialog/error-dialog.component';

@Component({
  selector: 'app-details-content',
  templateUrl: './details-content.component.html',
  styleUrls: ['./details-content.component.scss']
})
export class DetailsContentComponent {

  selectedLoader;
  selectedLinker;
  files = [];
  checkedFiles: TreeNode[] = [];

  constructor(public projectDetailsService: ProjectDetailsService, private projectService: ProjectService,
              private uploadService: UploadService, private snackBar: MatSnackBar,
              private loaderService: LoaderService, public dialog: MatDialog) {
  }

  onUploadFilesClick() {
    if (this.selectedLoader) {
      const projectId = this.projectDetailsService.project.getValue().projectId;

      this.uploadService.uploadModels(this.selectedLoader, projectId, this.files).subscribe(() => {
        this.projectService.getProject(projectId).subscribe(result => {
          if (result) {
            this.projectDetailsService.project.next(result);
            this.files = [];
          }
        }, (error: any) => {
          this.displayDialogErrorMessage('Could not get project', this.getErrorMessage(error));
        });
      }, (error: any) => {
        this.displayDialogErrorMessage('Could not upload files', this.getErrorMessage(error));
      });
    } else {
      this.displayDialogErrorMessage('You must select a loader first', '');
    }
  }

  onUpdateCheckedFiles(checkedNodes: TreeNode[]) {
    this.checkedFiles = checkedNodes;
  }

  onLoadFilesClick() {
    const projectId = this.projectDetailsService.project.getValue().projectId;

    this.loaderService.loadModels(projectId, this.checkedFiles).subscribe(() => {
      this.projectService.getProjectContext(projectId).subscribe(res => {
        if (res) {
          this.projectDetailsService.context.next(res);
        }
      }, (error: any) => {
        this.displayDialogErrorMessage('Could not get project context', this.getErrorMessage(error));
      });

      this.projectService.getProject(projectId).subscribe(result => {
        if (result) {
          this.projectDetailsService.project.next(result);
        }
      }, (error: any) => {
        this.displayDialogErrorMessage('Could not get project', this.getErrorMessage(error));
      });

    }, (error: any) => {
      console.log(error);
      this.displayDialogErrorMessage('Could not load models', this.getErrorMessage(error));
    });
  }

  onLinkButtonClick() {

  }

  onReloadModelsClick() {
    const projectId = this.projectDetailsService.project.getValue().projectId;

    this.loaderService.reloadProjectContext(projectId).subscribe(() => {
      this.projectService.getProjectContext(projectId).subscribe(res => {
        this.projectDetailsService.context.next(res);
      }, (error: any) => {
        this.displayDialogErrorMessage('Could not get project context', this.getErrorMessage(error));
      });
    }, (error: any) => {
      console.log(error);
      this.displayDialogErrorMessage('Could not reload project context', this.getErrorMessage(error));
    });
  }

  onClearContextButtonClick() {
    const projectId = this.projectDetailsService.project.getValue().projectId;

    this.loaderService.clearProjectContext(projectId).subscribe(() => {
      this.projectService.getProject(projectId).subscribe(result => {
        if (result) {
          this.projectDetailsService.project.next(result);
        }
      }, (error: any) => {
        this.displayDialogErrorMessage('Could not get project', this.getErrorMessage(error));
      });

      this.projectService.getProjectContext(projectId).subscribe(res => {
        this.projectDetailsService.context.next(res);
      }, (error: any) => {
        this.displayDialogErrorMessage('Could not get project context', this.getErrorMessage(error));
      });
    }, (error: any) => {
      this.displayDialogErrorMessage('Could not clear project context', this.getErrorMessage(error));
    });
  }

  displayDialogErrorMessage(mainProblem: string, errorMessage: string) {
    if (errorMessage == null || errorMessage === '') {
      this.dialog.open(ErrorDialogComponent, {data: {mainProblem: mainProblem, errorMessage: ''}});
    } else {
      this.dialog.open(ErrorDialogComponent, {data: {mainProblem: mainProblem, errorMessage: errorMessage}});
    }
  }

  private getErrorMessage(error: any) {
    return error.error?.detail ?? error.error;
  }
}
