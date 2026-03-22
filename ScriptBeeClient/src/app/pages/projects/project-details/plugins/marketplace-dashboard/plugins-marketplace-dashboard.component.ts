import { Component, computed, signal } from '@angular/core';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PluginService } from '../../../../../services/plugin/plugin.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';
import { PluginMarketplaceDashboardListComponent } from './list/plugin-marketplace-dashboard-list.component';

@Component({
  selector: 'app-plugins-marketplace-dashboard',
  templateUrl: './plugins-marketplace-dashboard.component.html',
  styleUrls: ['./plugins-marketplace-dashboard.component.scss'],
  providers: [],
  imports: [
    ErrorStateComponent,
    LoadingProgressBarComponent,
    PluginMarketplaceDashboardListComponent,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    FormsModule,
  ],
})
export class PluginsMarketplaceDashboardComponent {
  projectId = signal<string | undefined>(undefined);
  searchText = signal('');
  selectedFilters = signal<string[]>([]);

  getAllAvailablePlugins = createRxResourceHandler({
    loader: () => this.pluginsService.getAllAvailablePlugins(),
  });

  filteredPlugins = computed(() => {
    const plugins = this.getAllAvailablePlugins.value() ?? [];
    const search = this.searchText().toLowerCase();
    const filters = this.selectedFilters();

    const wantInstalled = filters.includes('installed');
    const wantNotInstalled = filters.includes('not-installed');
    const typeFilters = filters.filter((f) => f === 'plugins' || f === 'bundles');

    return plugins.filter((plugin) => {
      const matchesSearch = plugin.name.toLowerCase().includes(search) || plugin.description.toLowerCase().includes(search);

      const isInstalled = !!plugin.installedVersion;

      if (wantInstalled && !wantNotInstalled && !isInstalled) {
        return false;
      }
      if (wantNotInstalled && !wantInstalled && isInstalled) {
        return false;
      }

      if (typeFilters.length > 0) {
        if (plugin.type === 'Plugin' && !typeFilters.includes('plugins')) {
          return false;
        }
        if (plugin.type === 'Bundle' && !typeFilters.includes('bundles')) {
          return false;
        }
      }

      return matchesSearch;
    });
  });

  constructor(
    route: ActivatedRoute,
    private pluginsService: PluginService
  ) {
    let currentRoute: ActivatedRoute | null = route;
    while (currentRoute) {
      currentRoute.paramMap.pipe(takeUntilDestroyed()).subscribe((paramMap) => {
        if (paramMap.has('id') && !this.projectId()) {
          this.projectId.set(paramMap.get('id') ?? undefined);
        }
      });
      currentRoute = currentRoute.parent;
    }
  }

  onActionCompleted() {
    this.getAllAvailablePlugins.reload();
  }
}
