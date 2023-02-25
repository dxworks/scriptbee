import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { ScriptsService } from '../services/scripts.service';
import { catchError, EMPTY, pipe, tap } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { CreateScriptData, CreateScriptResponse, ScriptData, ScriptFileStructureNode, ScriptLanguage, UpdateScriptData } from '../services/script-types';

interface CreateScriptStoreState {
  availableLanguages: ScriptLanguage[];
  availableLanguagesError: ApiErrorMessage | undefined;

  createScriptResult: CreateScriptResponse | undefined;
  createScriptError: ApiErrorMessage | undefined;

  updateScriptResult: CreateScriptResponse | undefined;
  updateScriptError: ApiErrorMessage | undefined;

  scriptsForProject: ScriptFileStructureNode[] | undefined;
  scriptsForProjectError: ApiErrorMessage | undefined;

  scriptById: ScriptData | undefined;
  scriptByIdError: ApiErrorMessage | undefined;
}

@Injectable()
export class ScriptsStore extends ComponentStore<CreateScriptStoreState> {
  constructor(private scriptsService: ScriptsService) {
    super({
      availableLanguages: [],
      availableLanguagesError: undefined,
      createScriptResult: undefined,
      createScriptError: undefined,
      updateScriptResult: undefined,
      updateScriptError: undefined,
      scriptsForProject: undefined,
      scriptsForProjectError: undefined,
      scriptById: undefined,
      scriptByIdError: undefined,
    });
  }

  readonly availableLanguages = this.select((state) => state.availableLanguages);

  readonly createScriptResult = this.select((state) => state.createScriptResult);
  readonly createScriptError = this.select((state) => state.createScriptError);

  readonly updateScriptResult = this.select((state) => state.updateScriptResult);
  readonly updateScriptError = this.select((state) => state.updateScriptError);

  loadAvailableLanguages = this.effect<void>(
    pipe(
      switchMap(() =>
        this.scriptsService.getAvailableLanguages().pipe(
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
        this.scriptsService.createScript(createScriptData).pipe(
          tap({
            next: (createScriptResult) => this.patchState({ createScriptResult }),
            error: (error: HttpErrorResponse) => this.patchState({ createScriptError: { code: error.status, message: error.message } }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  updateScript = this.effect<UpdateScriptData>(
    pipe(
      switchMap((updateScriptData) =>
        this.scriptsService.updateScript(updateScriptData).pipe(
          tap({
            next: (updateScriptResult) => this.patchState({ updateScriptResult }),
            error: (error: HttpErrorResponse) => this.patchState({ updateScriptError: { code: error.status, message: error.message } }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  loadScriptsForProject = this.effect<string>(
    pipe(
      switchMap((projectId) =>
        this.scriptsService.getScripts(projectId).pipe(
          tap({
            next: (scriptsForProject) => this.patchState({ scriptsForProject }),
            error: (error: HttpErrorResponse) => this.patchState({ scriptsForProjectError: error.error }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  loadScriptById = this.effect<{ scriptId: string; projectId: string }>(
    pipe(
      switchMap(({ scriptId, projectId }) =>
        this.scriptsService.getScriptById(scriptId, projectId).pipe(
          tap({
            next: (scriptById) => this.patchState({ scriptById }),
            error: (error: HttpErrorResponse) => this.patchState({ scriptByIdError: error.error }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );
}
