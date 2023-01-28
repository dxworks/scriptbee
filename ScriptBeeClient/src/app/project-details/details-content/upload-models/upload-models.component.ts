import { Component, Input, OnInit } from '@angular/core';
import { Project } from "../../../state/project-details/project";
import { Store } from "@ngrx/store";
import { ErrorDialogService } from "../../../shared/error-dialog/error-dialog.service";
import { ProjectService } from "../../../services/project/project.service";
import { UploadService } from "../../../services/upload/upload.service";
import { setSavedFiles } from "../../../state/project-details/project-details.actions";
import { LoaderService } from "../../../services/loader/loader.service";
import { MatSnackBar } from "@angular/material/snack-bar";

@Component({
  selector: 'app-upload-models',
  templateUrl: './upload-models.component.html',
  styleUrls: ['./upload-models.component.scss']
})
export class UploadModelsComponent implements OnInit {

  @Input()
  project: Project | undefined;

  uploading = false;
  loaders: string[] = [];
  selectedLoader: string | undefined;
  files = [];

  constructor(private store: Store, private errorDialogService: ErrorDialogService,
    private projectService: ProjectService, private uploadService: UploadService,
    private loaderService: LoaderService, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.loaderService.getAllLoaders().subscribe({
      next: result => {
        this.loaders = result;
      },
      error: () => {
        this.snackBar.open('Could not get loaders!', 'Ok', {
          duration: 4000
        });
      }
    });
  }

  onUploadFilesClick() {
    if (!this.selectedLoader) {
      this.uploading = false;
      this.errorDialogService.displayDialogErrorMessage('You must select a loader first', '');
      return;
    }

    this.uploading = true;
    const projectId = this.project.data.projectId;

    this.uploadService.uploadModels(this.selectedLoader, projectId, this.files).subscribe({
      next: result => {
        this.store.dispatch(setSavedFiles({ files: result.files, loader: result.loaderName }))
        this.uploading = false;
        this.files = [];
      },
      error: err => {
        this.uploading = false;
        this.errorDialogService.displayDialogErrorMessage('Could not upload files', UploadModelsComponent.getErrorMessage(err));
      }
    });
  }

  // todo after errors are standardized, remove this
  private static getErrorMessage(error: any) {
    return error.error?.detail ?? error.error;
  }
}
