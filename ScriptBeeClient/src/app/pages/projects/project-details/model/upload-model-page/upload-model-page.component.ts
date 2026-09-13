import { Component, computed, inject, signal } from '@angular/core';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';
import { UploadService } from '../../../../../services/upload/upload.service';
import { finalize } from 'rxjs';
import { DragAndDropFilesComponent } from '../../../../../components/drag-and-drop-files/drag-and-drop-files.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProjectFile } from '../../../../../types/project';
import { HttpErrorResponse } from '@angular/common/http';
import { convertError } from '../../../../../utils/api';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-upload-model-page',
  imports: [DragAndDropFilesComponent, MatButtonModule, LoadingProgressBarComponent, MatListModule, MatIconModule, MatDividerModule, MatTooltipModule],
  templateUrl: './upload-model-page.component.html',
  styleUrl: './upload-model-page.component.scss',
})
export class UploadModelPage {
  private projectStateService = inject(ProjectStateService);
  private uploadService = inject(UploadService);
  private snackbar = inject(MatSnackBar);

  project = computed(() => this.projectStateService.currentProject()!);
  projectId = computed(() => this.projectStateService.currentProjectId()!);
  instanceId = computed(() => this.projectStateService.currentInstanceId());

  savedFiles = computed<ProjectFile[]>(() => this.project()?.savedFiles ?? []);

  isUploadLoading = signal(false);
  deletingFileId = signal<string | null>(null);

  files: File[] = [];

  onUploadFilesClick() {
    if (this.files.length === 0) {
      return;
    }

    this.isUploadLoading.set(true);
    this.uploadService
      .uploadFiles(this.projectId(), this.files)
      .pipe(finalize(() => this.isUploadLoading.set(false)))
      .subscribe({
        next: () => {
          this.files = [];
          this.projectStateService.reloadCurrentProject();
          this.snackbar.open('Files uploaded successfully', 'Dismiss', { duration: 4000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not upload files: ${error?.title ?? errorResponse.message}`, 'Dismiss', {
            duration: 4000,
          });
        },
      });
  }

  onDeleteFile(file: ProjectFile) {
    this.deletingFileId.set(file.id);
    this.uploadService
      .deleteSavedFile(this.projectId(), file.id)
      .pipe(finalize(() => this.deletingFileId.set(null)))
      .subscribe({
        next: () => {
          this.projectStateService.reloadCurrentProject();
          this.snackbar.open(`File '${file.name}' deleted successfully`, 'Dismiss', { duration: 4000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not delete file: ${error?.title ?? errorResponse.message}`, 'Dismiss', {
            duration: 4000,
          });
        },
      });
  }
}
