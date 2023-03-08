import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { catchError, EMPTY, pipe, switchMap, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { UploadService } from '../../services/upload/upload.service';
import { LoadersStore } from './loaders-store.service';

interface UploadModelsStoreState {
  files: File[];
  filesError: ApiErrorMessage | undefined;
  filesLoading: boolean;
}

@Injectable()
export class UploadModelsStore extends ComponentStore<UploadModelsStoreState> {
  constructor(private uploadService: UploadService, private loadersStore: LoadersStore) {
    super({
      files: [],
      filesError: undefined,
      filesLoading: false,
    });
  }

  readonly files = this.select((state) => state.files);
  readonly filesError = this.select((state) => state.filesError);
  readonly filesLoading = this.select((state) => state.filesLoading);

  uploadFiles = this.effect<{ projectId: string; loader: string; files: File[] }>(
    pipe(
      switchMap(({ projectId, loader, files }) => {
        this.patchState({ filesError: undefined, filesLoading: true });
        return this.uploadService.uploadModels(loader, projectId, files).pipe(
          tap({
            next: (uploadModelResult) => {
              this.loadersStore.updateSavedFiles([
                {
                  name: uploadModelResult.loaderName,
                  models: uploadModelResult.files,
                },
              ]);
              this.patchState({ filesLoading: false });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                filesError: { code: error.status, message: error.message },
                filesLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
