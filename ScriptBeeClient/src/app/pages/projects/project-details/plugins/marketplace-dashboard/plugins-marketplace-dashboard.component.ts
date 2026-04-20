import { Component, computed, inject, signal } from '@angular/core';
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
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { PluginMarketplaceDashboardListComponent } from './list/plugin-marketplace-dashboard-list.component';
import { convertError } from '../../../../../utils/api';
import { InstalledPlugin, MarketplacePlugin } from '../../../../../types/marketplace-plugin';
import { of } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';

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
    MatButtonModule,
    MatSnackBarModule,
    FormsModule,
    MatTooltipModule,
  ],
})
export class PluginsMarketplaceDashboardComponent {
  projectId = signal<string>('');
  searchText = signal('');
  selectedFilters = signal<string[]>([]);
  uploading = signal(false);

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

    const customPlugins: MarketplacePlugin[] = installedPlugins
      .filter((installed) => !plugins.some((p) => p.id === installed.id))
      .map(
        (installed) =>
          ({
            id: installed.id,
            name: installed.id,
            type: 'Plugin',
            description: 'Manually uploaded plugin',
            authors: ['Unknown'],
            versions: [{ version: installed.version, url: false, manifestUrl: '' }],
            isCustom: true,
          }) as MarketplacePlugin
      );

    const allPlugins = [...plugins, ...customPlugins];

    return allPlugins.filter((plugin) => {
      const matchesSearch = plugin.name.toLowerCase().includes(search) || plugin.description.toLowerCase().includes(search);

      const isInstalled = this.isPluginInInstalled(installedPlugins, plugin.id);

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

  private route = inject(ActivatedRoute);
  private pluginsService = inject(PluginService);
  private snackBar = inject(MatSnackBar);

  constructor() {
    let currentRoute: ActivatedRoute | null = this.route;
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

  handleFileUpload(event: Event) {
    const element = event.currentTarget as HTMLInputElement;
    const fileList: FileList | null = element.files;
    if (fileList && fileList.length > 0) {
      const file = fileList[0];
      this.uploading.set(true);
      this.pluginsService.uploadPlugin(this.projectId(), file).subscribe({
        next: () => {
          this.uploading.set(false);
          this.snackBar.open('Plugin uploaded successfully', 'Close', { duration: 3000 });
          this.onActionCompleted();
          element.value = '';
        },
        error: (err) => {
          this.uploading.set(false);
          const errorMessage = err.error?.detail ?? 'Failed to upload plugin';
          this.snackBar.open(errorMessage, 'Close', { duration: 5000 });
          element.value = '';
        },
      });
    }
  }

  private isPluginInInstalled(plugins: InstalledPlugin[], pluginId: string) {
    return plugins.some((plugin) => plugin.id === pluginId);
  }
}
