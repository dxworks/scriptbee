import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage, createApiErrorMessage } from '../../shared/api-error-message';
import { ScriptsService } from '../services/scripts.service';
import { catchError, EMPTY, pipe, tap } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { ScriptLanguage } from '../services/create-script';

interface CreateScriptStoreState {
  availableLanguages: ScriptLanguage[];
  availableLanguagesError: ApiErrorMessage | undefined;
}

@Injectable()
export class CreateScriptStore extends ComponentStore<CreateScriptStoreState> {
  constructor(private createScriptService: ScriptsService) {
    super({ availableLanguages: [], availableLanguagesError: undefined });
  }

  readonly availableLanguages = this.select((state) => state.availableLanguages);
  readonly availableLanguagesError = this.select((state) => state.availableLanguagesError);

  loadAvailableLanguages = this.effect<void>(
    pipe(
      switchMap(() =>
        this.createScriptService.getAvailableLanguages().pipe(
          tap({
            next: (availableLanguages) => this.patchState({ availableLanguages: availableLanguages }),
            error: (error) => this.patchState({ availableLanguagesError: createApiErrorMessage(error) }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );
}
