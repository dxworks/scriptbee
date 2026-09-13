import { Component, computed, inject, input, resource, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { LoaderService } from '../../../../../../services/instances/loader.service';
import { ProjectStateService } from '../../../../../../services/projects/project-state.service';
import { convertError } from '../../../../../../utils/api';
import { LoadingProgressBarComponent } from '../../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { Project } from '../../../../../../types/project';
import { Loader } from '../../../../../../types/load-model';
import { finalize, firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss'],
  imports: [MatButtonModule, MatSelectModule, MatFormFieldModule, MatCheckboxModule, MatIconModule, LoadingProgressBarComponent],
})
export class LoadModelsComponent {
  project = input.required<Project>();
  instanceId = input.required<string>();

  private loaderService = inject(LoaderService);
  private projectStateService = inject(ProjectStateService);
  private snackbar = inject(MatSnackBar);

  selectedLoaderId = signal<string | null>(null);
  selectedFileIds = signal<Set<string>>(new Set());
  isLoadModelsLoading = signal(false);
  unloadingLoaderId = signal<string | null>(null);
  unloadingFileId = signal<string | null>(null);

  loadersResource = resource({
    params: () => ({ projectId: this.project().id, instanceId: this.instanceId() }),
    loader: ({ params }) => firstValueFrom(this.loaderService.getAllLoaders(params.projectId, params.instanceId)),
  });

  loaders = computed<Loader[]>(() => this.loadersResource.value() ?? []);

  loadedFileEntries = computed(() => Object.entries(this.project().loadedFiles).filter(([, files]) => files.length > 0));

  onLoaderChange(loaderId: string | null) {
    this.selectedLoaderId.set(loaderId);
    this.selectedFileIds.set(new Set());
  }

  isFileSelected(fileId: string): boolean {
    return this.selectedFileIds().has(fileId);
  }

  toggleFile(fileId: string) {
    const current = new Set(this.selectedFileIds());
    if (current.has(fileId)) {
      current.delete(fileId);
    } else {
      current.add(fileId);
    }
    this.selectedFileIds.set(current);
  }

  onLoadFilesClick() {
    const loaderId = this.selectedLoaderId();
    if (!loaderId || this.selectedFileIds().size === 0) {
      return;
    }

    this.isLoadModelsLoading.set(true);
    const filesToLoad: Record<string, string[]> = {
      [loaderId]: [...this.selectedFileIds()],
    };

    this.loaderService
      .loadModels(this.project().id, this.instanceId(), filesToLoad)
      .pipe(finalize(() => this.isLoadModelsLoading.set(false)))
      .subscribe({
        next: () => {
          this.selectedFileIds.set(new Set());
          this.projectStateService.reloadCurrentProject();
          this.snackbar.open('Load successful', 'Dismiss', { duration: 4000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not load models: ${error?.title}`, 'Dismiss', {
            duration: 4000,
          });
        },
      });
  }

  onUnloadFile(loaderId: string, fileId: string) {
    this.unloadingFileId.set(fileId);
    this.loaderService
      .unloadModelFile(this.project().id, this.instanceId(), loaderId, fileId)
      .pipe(finalize(() => this.unloadingFileId.set(null)))
      .subscribe({
        next: () => {
          this.projectStateService.reloadCurrentProject();
          this.snackbar.open('File unloaded', 'Dismiss', { duration: 3000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not unload file: ${error?.title}`, 'Dismiss', {
            duration: 4000,
          });
        },
      });
  }

  onUnloadLoader(loaderId: string) {
    this.unloadingLoaderId.set(loaderId);
    this.loaderService
      .unloadLoader(this.project().id, this.instanceId(), loaderId)
      .pipe(finalize(() => this.unloadingLoaderId.set(null)))
      .subscribe({
        next: () => {
          this.projectStateService.reloadCurrentProject();
          this.snackbar.open('Loader unloaded', 'Dismiss', { duration: 3000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not unload loader: ${error?.title}`, 'Dismiss', {
            duration: 4000,
          });
        },
      });
  }
}
