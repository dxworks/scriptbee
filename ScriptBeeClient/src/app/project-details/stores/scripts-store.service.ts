import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { ScriptsService } from '../services/scripts.service';
import { catchError, EMPTY, pipe, tap } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ScriptData, ScriptFileStructureNode, ScriptLanguage } from '../services/script-types';

interface ScriptStoreState {
  availableLanguages: ScriptLanguage[];
  availableLanguagesError: ApiErrorMessage | undefined;

  scriptsForProject: ScriptFileStructureNode[] | undefined;
  scriptsForProjectError: ApiErrorMessage | undefined;
  scriptsForProjectLoading: boolean;

  scriptById: ScriptData | undefined;
  scriptByIdError: ApiErrorMessage | undefined;

  scriptContent: string | undefined;
  scriptContentError: ApiErrorMessage | undefined;
  scriptContentLoading: boolean;

  deleteScriptError: ApiErrorMessage | undefined;
  deleteScriptLoading: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ScriptsStore extends ComponentStore<ScriptStoreState> {
  constructor(private scriptsService: ScriptsService) {
    super({
      availableLanguages: [],
      availableLanguagesError: undefined,
      scriptsForProject: undefined,
      scriptsForProjectError: undefined,
      scriptsForProjectLoading: false,
      scriptById: undefined,
      scriptByIdError: undefined,
      scriptContent: undefined,
      scriptContentError: undefined,
      scriptContentLoading: false,
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

  getScriptParameters(projectId: string, scriptPath: string) {
    const script = this.getScriptRecursively(this.get().scriptsForProject, projectId, scriptPath);
    if (!script) {
      return [];
    }

    return script.scriptData.parameters;
  }

  updateScript(updateScriptResult: ScriptData) {
    this.patchState((state) => {
      const newScriptsForProject = state.scriptsForProject?.map((script) => {
        if (script.scriptData?.filePath === updateScriptResult.filePath && script.scriptData?.projectId === updateScriptResult.projectId) {
          return {
            ...script,
            scriptData: updateScriptResult,
          };
        }
        return script;
      });

      return {
        scriptsForProject: newScriptsForProject,
      };
    });
  }

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
        this.patchState({ deleteScriptLoading: true, deleteScriptError: undefined });
        return this.scriptsService.deleteScript(scriptId, projectId).pipe(
          tap({
            next: () =>
              this.patchState((state) => {
                const newScriptsForProject = state.scriptsForProject?.filter((script) => script.path !== scriptId);

                return { scriptsForProject: newScriptsForProject, deleteScriptLoading: false };
              }),
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

  private getScriptRecursively(scripts: ScriptFileStructureNode[], projectId: string, scriptPath: string): ScriptFileStructureNode | undefined {
    if (!scripts) {
      return undefined;
    }

    for (const script of scripts) {
      if (!script.children && script.scriptData) {
        if (script.scriptData.filePath === scriptPath && script.scriptData.projectId === projectId) {
          return script;
        }
      } else {
        const result = this.getScriptRecursively(script.children, projectId, scriptPath);
        if (result) {
          return result;
        }
      }
    }
    return undefined;
  }
}
