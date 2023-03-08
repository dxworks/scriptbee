import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { catchError, EMPTY, of, pipe, switchMap, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { TreeNode, updateTreeNodeArray } from '../../shared/tree-node';
import { LoaderService } from '../../services/loader/loader.service';
import { ReturnedContextSlice } from '../services/returned-context-slice';

interface LoadersStoreState {
  loaders: string[];
  loadersError: ApiErrorMessage | undefined;
  loadersLoading: boolean;

  savedFiles: TreeNode[];
  loadedFiles: TreeNode[];

  loadedModels: TreeNode[];
  loadedModelsError: ApiErrorMessage | undefined;
  loadedModelsLoading: boolean;
}

@Injectable()
export class LoadersStore extends ComponentStore<LoadersStoreState> {
  constructor(private loaderService: LoaderService) {
    super({
      loaders: [],
      loadersError: undefined,
      loadersLoading: false,
      savedFiles: [],
      loadedFiles: [],
      loadedModels: [],
      loadedModelsError: undefined,
      loadedModelsLoading: false,
    });
  }

  readonly loaders = this.select((state) => state.loaders);
  readonly loadersError = this.select((state) => state.loadersError);
  readonly loadersLoading = this.select((state) => state.loadersLoading);

  readonly savedFiles = this.select((state) => state.savedFiles);
  readonly loadedFiles = this.select((state) => state.loadedFiles);

  readonly loadModelsError = this.select((state) => state.loadedModelsError);
  readonly loadModelsLoading = this.select((state) => state.loadedModelsLoading);

  setSavedFiles = this.updater((state, savedFiles: TreeNode[]) => ({ ...state, savedFiles: savedFiles }));

  setLoadedFiles = this.effect<TreeNode[]>(
    pipe(
      switchMap((loadedFiles) =>
        of(loadedFiles).pipe(
          tap({
            next: (loadedFiles) => {
              this.patchState({ loadedFiles: [...loadedFiles] });
            },
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  updateSavedFiles = this.effect<ReturnedContextSlice[]>(
    pipe(
      switchMap((contextSlices) =>
        of(contextSlices).pipe(
          tap({
            next: (contextSlices) => {
              let savedFiles: TreeNode[] = this.get().savedFiles;
              for (const loadModelsResult of contextSlices) {
                savedFiles = updateTreeNodeArray(savedFiles, loadModelsResult.name, loadModelsResult.models);
              }

              this.patchState({ savedFiles: [...savedFiles] });
            },
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  updateLoadedFiles = this.effect<TreeNode[]>(
    pipe(
      switchMap((models) =>
        of(models).pipe(
          tap({
            next: (models) => {
              this.patchState({ loadedFiles: [...models] });
            },
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  loadLoaders = this.effect<void>(
    pipe(
      switchMap(() => {
        this.patchState({ loadersError: undefined, loadersLoading: true });
        return this.loaderService.getAllLoaders().pipe(
          tap({
            next: (loaders) => {
              this.patchState({ loaders, loadersLoading: false });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                loadersError: { code: error.status, message: error.message },
                loadersLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );

  loadModels = this.effect<{ projectId: string; models: TreeNode[] }>(
    pipe(
      switchMap(({ projectId, models }) => {
        this.patchState({ loadedModelsError: undefined, loadedModelsLoading: true });
        return this.loaderService.loadModels(projectId, models).pipe(
          tap({
            next: () => {
              this.updateLoadedFiles(models);
              this.patchState({ loadedModelsLoading: false });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                loadedModelsError: { code: error.status, message: error.message },
                loadedModelsLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
