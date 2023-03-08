import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { ScriptsService } from '../services/scripts.service';
import { catchError, EMPTY, pipe, tap } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { CreateScriptData, CreateScriptResponse } from '../services/script-types';
import { ScriptsStore } from './scripts-store.service';

interface CreateScriptStoreState {
  createScriptResult: CreateScriptResponse | undefined;
  createScriptError: ApiErrorMessage | undefined;
}

@Injectable()
export class CreateScriptStore extends ComponentStore<CreateScriptStoreState> {
  constructor(private scriptsService: ScriptsService, private scriptsStore: ScriptsStore) {
    super({
      createScriptResult: undefined,
      createScriptError: undefined,
    });
  }

  readonly createScriptResult = this.select((state) => state.createScriptResult);
  readonly createScriptError = this.select((state) => state.createScriptError);

  createScript = this.effect<CreateScriptData>(
    pipe(
      switchMap((createScriptData) =>
        this.scriptsService.createScript(createScriptData).pipe(
          tap({
            next: (createScriptResult) => {
              this.scriptsStore.loadScriptsForProject(createScriptData.projectId);
              this.patchState({ createScriptResult });
            },
            error: (error: HttpErrorResponse) => this.patchState({ createScriptError: { code: error.status, message: error.message } }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );
}
