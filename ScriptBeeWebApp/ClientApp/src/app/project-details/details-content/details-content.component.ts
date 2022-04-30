import {Component} from '@angular/core';
import {ProjectService} from '../../services/project/project.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ProjectDetailsService} from '../project-details.service';
import {TreeNode} from "../../shared/tree-node";
import {UploadService} from "../../services/upload/upload.service";
import {forkJoin} from "rxjs";
import {LoaderService} from "../../services/loader/loader.service";

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
              private loaderService: LoaderService) {
  }

  onUploadFilesClick() {
    if (this.selectedLoader) {
      const projectId = this.projectDetailsService.project.getValue().projectId;

      this.uploadService.uploadModels(this.selectedLoader, projectId, this.files).subscribe(() => {
        this.projectService.getProjectContext(projectId).subscribe(res => {
          this.projectDetailsService.context.next(res);
          this.files = [];
        });

        this.projectService.getProject(projectId).subscribe(result => {
          if (result) {
            this.projectDetailsService.project.next(result);
            console.log(result);
          }
        }, (error: any) => {
          this.snackBar.open('Could not get project!', 'Ok', {
            duration: 4000
          });
        });

      }, (error: any) => {
        this.snackBar.open('Could not upload files!', 'Ok', {
          duration: 4000
        });
      });
    } else {
      this.snackBar.open('You must select a loader first!', 'Ok', {
        duration: 4000
      });
    }
  }

  onUpdateCheckedFiles(checkedNodes: TreeNode[]) {
    this.checkedFiles = checkedNodes;
  }

  onLoadFilesClick() {
    const projectId = this.projectDetailsService.project.getValue().projectId;

    this.loaderService.loadModels(projectId, this.checkedFiles).subscribe(()=> {
      this.projectService.getProjectContext(projectId).subscribe(res => {
        this.projectDetailsService.context.next(res);
        console.log(res);
      });

      this.projectService.getProject(projectId).subscribe(result => {
        if (result) {
          this.projectDetailsService.project.next(result);
          console.log(result);
        }
      }, (error: any) => {
        this.snackBar.open('Could not get project!', 'Ok', {
          duration: 4000
        });
      });

    }, (error: any) => {
      this.snackBar.open('Could not load files!', 'Ok', {
        duration: 4000
      });
    });
  }

  onLinkButtonClick() {

  }
}
