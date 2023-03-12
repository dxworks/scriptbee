import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { catchError, EMPTY, pipe, switchMap, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { LoaderService } from '../../services/loader/loader.service';
import { ProjectService } from '../services/project.service';
import { TreeNode } from '../../shared/tree-node';

interface ContextStoreState {
  context: TreeNode[];
  contextError: ApiErrorMessage | undefined;
  contextLoading: boolean;

  reloadingContext: boolean;
  reloadingContextError: ApiErrorMessage | undefined;

  clearingContext: boolean;
  clearingContextError: ApiErrorMessage | undefined;
}

@Injectable()
export class ContextStore extends ComponentStore<ContextStoreState> {
  constructor(private projectService: ProjectService, private loaderService: LoaderService) {
    super({
      context: [],
      contextError: undefined,
      contextLoading: false,
      reloadingContext: false,
      reloadingContextError: undefined,
      clearingContext: false,
      clearingContextError: undefined,
    });
  }

  readonly context = this.select((state) => state.context);
  readonly contextError = this.select((state) => state.contextError);
  readonly contextLoading = this.select((state) => state.contextLoading);

  readonly reloadingContext = this.select((state) => state.reloadingContext);
  readonly reloadingContextError = this.select((state) => state.reloadingContextError);

  readonly clearingContext = this.select((state) => state.clearingContext);
  readonly clearingContextError = this.select((state) => state.clearingContextError);

  loadContext = this.effect<{ projectId: string }>(
    pipe(
      switchMap(({ projectId }) => {
        this.patchState({ contextError: undefined, contextLoading: true });
        return this.projectService.getProjectContext(projectId).pipe(
          tap({
            next: (context) => {
              this.patchState({ context: context, contextLoading: false });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                contextError: { code: error.status, message: error.message },
                contextLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );

  reloadContext = this.effect<{ projectId: string }>(
    pipe(
      switchMap(({ projectId }) => {
        this.patchState({ reloadingContextError: undefined, reloadingContext: true });
        return this.loaderService.reloadProjectContext(projectId).pipe(
          tap({
            next: (contextSlices) => {
              const context: TreeNode[] = [];
              contextSlices.forEach((contextSlice) => {
                context.push({
                  name: contextSlice.name,
                  children: contextSlice.models.map((model) => ({ name: model })),
                });
              });

              this.patchState({ reloadingContext: false, context });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                reloadingContextError: { code: error.status, message: error.message },
                reloadingContext: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );

  clearContext = this.effect<{ projectId: string }>(
    pipe(
      switchMap(({ projectId }) => {
        this.patchState({ clearingContextError: undefined, clearingContext: true });
        return this.loaderService.clearProjectContext(projectId).pipe(
          tap({
            next: () => {
              this.patchState({ clearingContext: false, context: [] });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                clearingContextError: { code: error.status, message: error.message },
                clearingContext: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
