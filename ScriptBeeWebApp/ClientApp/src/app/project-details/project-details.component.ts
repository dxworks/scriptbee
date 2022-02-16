import {Component, OnInit} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {ProjectService} from '../services/project/project.service';
import {ActivatedRoute} from '@angular/router';
import {Project} from '../projects/project';
import {MatSnackBar} from '@angular/material/snack-bar';
import {TreeNode} from '../shared/tree/tree.component';
import {LoaderService} from '../services/loader/loader.service';
import {UploadService} from "../services/upload/upload.service";

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.scss']
})
export class ProjectDetailsComponent implements OnInit {

  project: Project;
  loaders = [];
  treeData: TreeNode[] = [];
  selectedLoader;
  files = [];

  constructor(public dialog: MatDialog, private projectService: ProjectService, private loaderService: LoaderService,
              private uploadService: UploadService, private route: ActivatedRoute, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    this.projectService.getProject(id).subscribe(result => {
      if (result) {
        this.project = result;
      }
    }, (error: any) => {
      this.snackBar.open('Could not get project!', 'Ok', {
        duration: 4000
      });
    });

    this.loaderService.getAllLoaders().subscribe(result => {
      if (result) {
        this.loaders = result;
      }
    }, (error: any) => {
      this.snackBar.open('Could not get loaders!', 'Ok', {
        duration: 4000
      });
    });

    this.projectService.getProjectContext(id).subscribe(result => {
      this.treeData = result;
    });
  }

  onUploadFilesClick() {
    if (this.selectedLoader) {
      this.uploadService.uploadModels(this.selectedLoader, this.project.projectId, this.files).subscribe(result => {
        this.projectService.getProjectContext(this.project.projectId).subscribe(result => {
          this.treeData = result;
        });
        this.files = [];
      });
    } else {
      this.snackBar.open('You must select a loader first!', 'Ok', {
        duration: 4000
      });
    }
  }
}
