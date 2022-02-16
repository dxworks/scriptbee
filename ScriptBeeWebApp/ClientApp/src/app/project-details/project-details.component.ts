import {Component, OnInit} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {ProjectService} from '../services/project/project.service';
import {ActivatedRoute} from '@angular/router';
import {Project} from '../projects/project';
import {MatSnackBar} from '@angular/material/snack-bar';
import {TreeNode} from '../shared/tree/tree.component';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.scss']
})
export class ProjectDetailsComponent implements OnInit {

  project: Project;
  loaders = ["valoare 1", "valoare 2"];
  treeData: TreeNode[] = [];
  selectedLoader ;

  constructor(public dialog: MatDialog, private projectService: ProjectService,
              private route: ActivatedRoute, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    this.projectService.getProject(id).subscribe(result => {
      if (result) {
        console.log(result);
        this.project = result;
      }
    }, (error: any) => {
      this.snackBar.open('Could not get project!', 'Ok', {
        duration: 4000
      });
    });

    this.projectService.getProjectContext(id).subscribe(result => {
      this.treeData = [];
    });
  }

  onUploadFilesClick() {
    if(this.selectedLoader) {
      console.log(this.selectedLoader);
    }
    else {
      this.snackBar.open("You must select a loader first!", 'Ok', {
        duration: 5000
      });
    }
  }
}
