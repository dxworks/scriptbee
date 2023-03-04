import {Injectable} from '@angular/core';
import {ComponentStore} from '@ngrx/component-store';
import {ApiErrorMessage} from '../../shared/api-error-message';
import {ScriptsService} from '../services/scripts.service';
import {catchError, EMPTY, pipe, tap} from 'rxjs';
import {switchMap} from 'rxjs/operators';
import {HttpErrorResponse} from '@angular/common/http';
import {
    CreateScriptData,
    CreateScriptResponse,
    ScriptData,
    ScriptFileStructureNode,
    ScriptLanguage,
    UpdateScriptData
} from '../services/script-types';

interface CreateScriptStoreState {
  availableLanguages: ScriptLanguage[];
  availableLanguagesError: ApiErrorMessage | undefined;

  createScriptResult: CreateScriptResponse | undefined;
  createScriptError: ApiErrorMessage | undefined;

  updateScriptResult: CreateScriptResponse | undefined;
  updateScriptError: ApiErrorMessage | undefined;

  scriptsForProject: ScriptFileStructureNode[] | undefined;
  scriptsForProjectError: ApiErrorMessage | undefined;
  scriptsForProjectLoading: boolean;

  scriptById: ScriptData | undefined;
  scriptByIdError: ApiErrorMessage | undefined;

  scriptContent: string | undefined;
  scriptContentError: ApiErrorMessage | undefined;
  scriptContentLoading: boolean;

  deleteScriptResult: string | undefined;
  deleteScriptError: ApiErrorMessage | undefined;
  deleteScriptLoading: boolean;
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
      scriptsForProjectLoading: false,
      scriptById: undefined,
      scriptByIdError: undefined,
      scriptContent: undefined,
      scriptContentError: undefined,
      scriptContentLoading: false,
      deleteScriptResult: undefined,
      deleteScriptError: undefined,
      deleteScriptLoading: false,
    });
  }

  readonly availableLanguages = this.select((state) => state.availableLanguages);

  readonly scriptsForProject = this.select((state) => state.scriptsForProject);
  readonly scriptsForProjectError = this.select((state) => state.scriptsForProjectError);
  readonly scriptsForProjectLoading = this.select((state) => state.scriptsForProjectLoading);

  readonly scriptById = this.select((state) => state.scriptById);
  readonly scriptByIdError = this.select((state) => state.scriptByIdError);

  readonly scriptContent = this.select((state) => state.scriptContent);
  readonly scriptContentError = this.select((state) => state.scriptContentError);
  readonly scriptContentLoading = this.select((state) => state.scriptContentLoading);

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
            error: (error: HttpErrorResponse) =>
              this.patchState({
                availableLanguagesError: {
                  code: error.status,
                  message: error.message,
                },
              }),
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
      switchMap((projectId) => {
        this.patchState({ scriptsForProjectLoading: true, scriptsForProjectError: undefined, scriptsForProject: undefined });
        return this.scriptsService.getScripts(projectId).pipe(
          tap({
            next: (scriptsForProject) => this.patchState({ scriptsForProject, scriptsForProjectLoading: false }),
            error: (error: HttpErrorResponse) =>
              this.patchState({
                scriptsForProjectError: { code: error.status, message: error.message },
                scriptsForProjectLoading: false,
              }),
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );

  loadScriptById = this.effect<{ scriptId: string; projectId: string }>(
    pipe(
      switchMap(({ scriptId, projectId }) =>
        this.scriptsService.getScriptById(scriptId, projectId).pipe(
          tap({
            next: (scriptById) => this.patchState({ scriptById }),
            error: (error: HttpErrorResponse) => this.patchState({ scriptByIdError: { code: error.status, message: error.message } }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );

  loadScriptContent = this.effect<{ scriptId: string; projectId: string }>(
    pipe(
      switchMap(({ scriptId, projectId }) => {
        this.patchState({ scriptContentLoading: true, scriptContentError: undefined, scriptContent: undefined });
        return this.scriptsService.getScriptContent(scriptId, projectId).pipe(
          tap({
            next: (scriptContent) => this.patchState({ scriptContent, scriptContentLoading: false }),
            error: (error: HttpErrorResponse) =>
              this.patchState({
                scriptContentError: { code: error.status, message: error.message },
                scriptContentLoading: false,
              }),
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );

  deleteScript = this.effect<{ scriptId: string; projectId: string }>(
    pipe(
      switchMap(({ scriptId, projectId }) => {
        this.patchState({ deleteScriptLoading: true, deleteScriptError: undefined, deleteScriptResult: undefined });
        return this.scriptsService.deleteScript(scriptId, projectId).pipe(
          tap({
            next: () => this.patchState({ deleteScriptResult: 'SUCCESS', deleteScriptLoading: false }),
            error: (error: HttpErrorResponse) =>
              this.patchState({
                deleteScriptError: { code: error.status, message: error.message },
                deleteScriptLoading: false,
              }),
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
