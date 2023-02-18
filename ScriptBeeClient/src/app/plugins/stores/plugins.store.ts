import { ComponentStore } from '@ngrx/component-store';
import { Injectable } from '@angular/core';
import { catchError, EMPTY, pipe, switchMap, tap } from 'rxjs';
import { PluginService } from '../services/plugin.service';
import { MarketplaceProject } from '../services/marketplace-project';
import { ApiErrorMessage, createApiErrorMessage } from '../../shared/api-error-message';

interface PluginsStoreState {
  marketPlacePlugins: MarketplaceProject[] | undefined;
  marketplacePluginsError: ApiErrorMessage | undefined;
}

@Injectable()
export class PluginsStore extends ComponentStore<PluginsStoreState> {
  constructor(private pluginService: PluginService) {
    super({ marketPlacePlugins: undefined, marketplacePluginsError: undefined });
  }

  readonly marketPlacePlugins = this.select((state) => state.marketPlacePlugins);
  readonly marketplacePluginsError = this.select((state) => state.marketplacePluginsError);

  loadMarketplacePlugins = this.effect<void>(
    pipe(
      switchMap(() =>
        this.pluginService.getAllAvailablePlugins().pipe(
          tap({
            next: (marketPlacePlugins) => this.patchState({ marketPlacePlugins: marketPlacePlugins }),
            error: (error) => this.patchState({ marketplacePluginsError: createApiErrorMessage(error) }),
          }),
          catchError(() => EMPTY)
        )
      )
    )
  );
}
