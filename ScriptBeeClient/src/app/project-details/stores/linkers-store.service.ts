import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { LinkerService } from '../../services/linker/linker.service';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { catchError, EMPTY, pipe, switchMap, tap } from 'rxjs';

interface LinkersStoreState {
  linkers: string[];
  linkersError: ApiErrorMessage | undefined;
  linkersLoading: boolean;

  linkModelsError: ApiErrorMessage | undefined;
  linkModelsLoading: boolean;
}

@Injectable()
export class LinkersStore extends ComponentStore<LinkersStoreState> {
  constructor(private linkersService: LinkerService) {
    super({
      linkers: [],
      linkersError: undefined,
      linkersLoading: false,
      linkModelsError: undefined,
      linkModelsLoading: false,
    });
  }

  readonly linkers = this.select((state) => state.linkers);
  readonly linkersError = this.select((state) => state.linkersError);
  readonly linkersLoading = this.select((state) => state.linkersLoading);

  readonly linkModelsError = this.select((state) => state.linkModelsError);
  readonly linkModelsLoading = this.select((state) => state.linkModelsLoading);

  loadLinkers = this.effect<void>(
    pipe(
      switchMap(() => {
        this.patchState({ linkers: [], linkersError: undefined, linkersLoading: true });
        return this.linkersService.getAllLinkers().pipe(
          tap({
            next: (linkers) => {
              this.patchState({ linkers, linkersLoading: false });
            },
            error: (error: ApiErrorMessage) => {
              this.patchState({
                linkersError: error,
                linkersLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );

  linkModels = this.effect<{ projectId: string; linkerName: string }>(
    pipe(
      switchMap(({ projectId, linkerName }) => {
        this.patchState({ linkModelsError: undefined, linkModelsLoading: true });
        return this.linkersService.linkModels(projectId, linkerName).pipe(
          tap({
            next: () => {
              this.patchState({ linkModelsLoading: false });
            },
            error: (error: ApiErrorMessage) => {
              this.patchState({
                linkModelsError: error,
                linkModelsLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
