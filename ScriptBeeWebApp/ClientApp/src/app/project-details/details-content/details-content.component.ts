import {Component} from '@angular/core';
import {ProjectService} from '../../services/project/project.service';
import {UploadService} from '../../services/upload/upload.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ProjectDetailsService} from '../project-details.service';
import {TreeNode} from "../../shared/tree-node";

@Component({
  selector: 'app-details-content',
  templateUrl: './details-content.component.html',
  styleUrls: ['./details-content.component.scss']
})
export class DetailsContentComponent {

  selectedLoader;
  selectedLinker;
  files = [];

  constructor(public projectDetailsService: ProjectDetailsService, private projectService: ProjectService,
              private uploadService: UploadService, private snackBar: MatSnackBar) {
  }

  onUploadFilesClick() {
    if (this.selectedLoader) {
      const projectId = this.projectDetailsService.project.getValue().projectId;
      this.uploadService.uploadModels(this.selectedLoader, projectId, this.files).subscribe(() => {
        this.projectService.getProjectContext(projectId).subscribe(res => {
          this.projectDetailsService.context.next(res);
        });
        this.files = [];
      });
    } else {
      this.snackBar.open('You must select a loader first!', 'Ok', {
        duration: 4000
      });
    }
  }

  onUpdateCheckNodes(checkedNodes: TreeNode[]) {
    console.log(checkedNodes);
  }

  onLoadFilesClick(){

  }

  onLinkButtonClick() {

  }
}
