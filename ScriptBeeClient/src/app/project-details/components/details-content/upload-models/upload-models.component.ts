import { Component, OnInit } from '@angular/core';
import { ErrorDialogService } from '../../../../shared/error-dialog/error-dialog.service';
import { ProjectStore } from '../../../stores/project-store.service';
import { LoadersStore } from '../../../stores/loaders-store.service';
import { UploadModelsStore } from '../../../stores/upload-models-store.service';

@Component({
  selector: 'app-upload-models',
  templateUrl: './upload-models.component.html',
  styleUrls: ['./upload-models.component.scss'],
})
export class UploadModelsComponent implements OnInit {
  loaders$ = this.loadersStore.loaders;
  loadersLoading$ = this.loadersStore.loadersLoading;
  loadersError$ = this.loadersStore.loadersError;

  filesLoading$ = this.uploadModelsStore.filesLoading;

  selectedLoader: string | undefined;
  files = [];
  private projectId: string;

  constructor(
    private projectStore: ProjectStore,
    private loadersStore: LoadersStore,
    private uploadModelsStore: UploadModelsStore,
    private errorDialogService: ErrorDialogService
  ) {}

  ngOnInit(): void {
    this.projectId = this.projectStore.getProjectId();

    this.loadersStore.loadLoaders();

    this.uploadModelsStore.files.subscribe((files) => {
      this.files = files;
    });

    this.uploadModelsStore.filesError.subscribe((error) => {
      if (error) {
        this.errorDialogService.displayDialogErrorMessage('Could not upload files', error.message);
      }
    });
  }

  onUploadFilesClick() {
    if (!this.selectedLoader) {
      this.errorDialogService.displayDialogErrorMessage('You must select a loader first', '');
      return;
    }

    this.uploadModelsStore.uploadFiles({ projectId: this.projectId, loader: this.selectedLoader, files: this.files });
  }
}
