import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { ScriptsService } from '../services/scripts.service';
import { catchError, EMPTY, pipe, tap } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { CreateScriptResponse, UpdateScriptData } from '../services/script-types';
import { ScriptsStore } from './scripts-store.service';

interface UpdateScriptStoreState {
  updateScriptResult: CreateScriptResponse | undefined;
  updateScriptError: ApiErrorMessage | undefined;
  updateScriptLoading: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class UpdateScriptStore extends ComponentStore<UpdateScriptStoreState> {
  constructor(private scriptsService: ScriptsService, private scriptsStore: ScriptsStore) {
    super({
      updateScriptResult: undefined,
      updateScriptError: undefined,
      updateScriptLoading: false,
    });
  }

  readonly updateScriptResult = this.select((state) => state.updateScriptResult);
  readonly updateScriptError = this.select((state) => state.updateScriptError);
  readonly updateScriptLoading = this.select((state) => state.updateScriptLoading);

  updateScript = this.effect<UpdateScriptData>(
    pipe(
      switchMap((updateScriptData) => {
        this.patchState({ updateScriptLoading: true, updateScriptError: undefined, updateScriptResult: undefined });
        return this.scriptsService.updateScript(updateScriptData).pipe(
          tap({
            next: (updateScriptResult) => {
              this.scriptsStore.updateScript(updateScriptResult);
              this.patchState({ updateScriptResult, updateScriptLoading: false });
            },
            error: (error: HttpErrorResponse) =>
              this.patchState({
                updateScriptError: { code: error.status, message: error.message },
                updateScriptLoading: false,
              }),
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
