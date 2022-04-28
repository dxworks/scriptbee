import {Component} from '@angular/core';
import {ProjectService} from '../../services/project/project.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ProjectDetailsService} from '../project-details.service';
import {TreeNode} from "../../shared/tree-node";
import {UploadService} from "../../services/upload/upload.service";
import {SaveFilesService} from "../../services/save-files/save-files.service";
import {forkJoin} from "rxjs";

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
  availableFiles: TreeNode[] = [];

  constructor(public projectDetailsService: ProjectDetailsService, private projectService: ProjectService,
              private uploadService: UploadService, private snackBar: MatSnackBar,
              private saveFilesService: SaveFilesService) {
  }

  onUploadFilesClick() {
    if (this.selectedLoader) {
      this.saveFilesService.updateSavedFilesDictionary(this.selectedLoader, this.files);
      this.files = [];
      this.availableFiles = this.saveFilesService.getSavedFiles();
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
    const observables = this.checkedFiles.map(node => this.uploadService.uploadModels(node.name, projectId, node.children));

    forkJoin(observables).subscribe(() => {
      this.projectService.getProjectContext(projectId).subscribe(res => {
        this.projectDetailsService.context.next(res);
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
