import {Injectable} from '@angular/core';
import {ComponentStore} from '@ngrx/component-store';
import {ApiErrorMessage} from '../../shared/api-error-message';
import {ScriptsService} from '../services/scripts.service';
import {catchError, EMPTY, pipe, tap} from 'rxjs';
import {switchMap} from 'rxjs/operators';
import {HttpErrorResponse} from '@angular/common/http';
import {CreateScriptData, CreateScriptResponse, ScriptLanguage} from '../services/script-types';

interface CreateScriptStoreState {
  availableLanguages: ScriptLanguage[];
  availableLanguagesError: ApiErrorMessage | undefined;

  createScriptResult: CreateScriptResponse | undefined;
  createScriptError: ApiErrorMessage | undefined;
}

@Injectable()
export class CreateScriptStore extends ComponentStore<CreateScriptStoreState> {
  constructor(private createScriptService: ScriptsService) {
    super({
      availableLanguages: [],
      availableLanguagesError: undefined,
      createScriptResult: undefined,
      createScriptError: undefined,
    });
  }

  readonly availableLanguages = this.select((state) => state.availableLanguages);

  readonly createScriptResult = this.select((state) => state.createScriptResult);
  readonly createScriptError = this.select((state) => state.createScriptError);

  loadAvailableLanguages = this.effect<void>(
    pipe(
      switchMap(() =>
        this.createScriptService.getAvailableLanguages().pipe(
          tap({
            next: (availableLanguages) => this.patchState({ availableLanguages }),
            error: (error: HttpErrorResponse) => this.patchState({ availableLanguagesError: error.error }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  createScript = this.effect<CreateScriptData>(
    pipe(
      switchMap((createScriptData) =>
        this.createScriptService.createScript(createScriptData).pipe(
          tap({
            next: (createScriptResult) => this.patchState({ createScriptResult }),
            error: (error: HttpErrorResponse) => this.patchState({ createScriptError: { code: error.status, message: error.message } }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );
}
