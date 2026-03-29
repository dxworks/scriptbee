import { Component, computed, signal } from '@angular/core';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { PluginService } from '../../../../../services/plugin/plugin.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';
import { PluginMarketplaceDashboardListComponent } from './list/plugin-marketplace-dashboard-list.component';
import { convertError } from '../../../../../utils/api';
import { InstalledPlugin } from '../../../../../types/marketplace-plugin';
import { Observable, of } from 'rxjs';

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
  projectId = signal<string>('');
  searchText = signal('');
  selectedFilters = signal<string[]>([]);

  getAllAvailablePlugins = rxResource({
    stream: () => this.pluginsService.getAllAvailablePlugins(),
  });

  installedPlugins = rxResource({
    params: () => this.projectId(),
    stream: ({ params: projectId }) => {
      if (projectId) {
        return this.pluginsService.getInstalledPlugins(projectId);
      }
      return of([]);
    },
  });

  getAllAvailablePluginsError = computed(() => convertError(this.getAllAvailablePlugins.error()));

  filteredPlugins = computed(() => {
    const plugins = this.getAllAvailablePlugins.value() ?? [];
    const installedPlugins = this.installedPlugins.value() ?? [];
    const search = this.searchText().toLowerCase();
    const filters = this.selectedFilters();

    const wantInstalled = filters.includes('installed');
    const wantNotInstalled = filters.includes('not-installed');
    const typeFilters = filters.filter((f) => f === 'plugins' || f === 'bundles');

    return plugins.filter((plugin) => {
      const matchesSearch = plugin.name.toLowerCase().includes(search) || plugin.description.toLowerCase().includes(search);

      const isInstalled = this.isPluginInInstalled(installedPlugins, plugin.versions[plugin.versions.length - 1].version);

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
          this.projectId.set(paramMap.get('id') ?? (undefined as unknown as string));
        }
      });
      currentRoute = currentRoute.parent;
    }
  }

  onActionCompleted() {
    this.getAllAvailablePlugins.reload();
    this.installedPlugins.reload();
  }

  private isPluginInInstalled(plugins: InstalledPlugin[], version: string) {
    return plugins.some((plugin) => plugin.version === version);
  }
}
