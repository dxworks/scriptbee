import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {CreateProjectDialogData} from './create-project-dialog-data';
import {SlugifyPipe} from "../../shared/slugify.pipe";
import {ProjectService} from "../../services/project/project.service";

@Component({
  selector: 'app-create-project-dialog',
  templateUrl: './create-project-dialog.component.html',
  styleUrls: ['./create-project-dialog.component.scss']
})
export class CreateProjectDialogComponent {

  projectExists = false;

  constructor(public dialogRef: MatDialogRef<CreateProjectDialogComponent>, private projectService: ProjectService,
              @Inject(MAT_DIALOG_DATA) public data: CreateProjectDialogData, public slugify: SlugifyPipe) {
    if (!data || !data.projectName) {
      this.data = {projectName: ''};
    }
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    this.projectService.createProject(this.slugify.transform(this.data.projectName), this.data.projectName).subscribe(res => {
      if (res) {
        this.dialogRef.close(true);
      }
    }, (error: any) => {
      this.projectExists = true;
    });
  }
}
