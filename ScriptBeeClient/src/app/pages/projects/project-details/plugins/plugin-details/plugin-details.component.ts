import { Component, computed, inject, signal } from '@angular/core';

import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { PluginService } from '../../../../../services/plugin/plugin.service';
import { PluginDetailsHeaderComponent } from './components/plugin-details-header/plugin-details-header.component';
import { PluginDetailsVersionsTableComponent } from './components/plugin-details-versions-table/plugin-details-versions-table.component';
import { PluginDetailsBundleNavigatorComponent } from './components/plugin-details-bundle-navigator/plugin-details-bundle-navigator.component';
import { PluginDetailsInfoComponent } from './components/plugin-details-info/plugin-details-info.component';
import { PluginDetailsExtensionsComponent } from './components/plugin-details-extensions/plugin-details-extensions.component';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { convertError } from '../../../../../utils/api';

@Component({
  selector: 'app-plugin-details',
  templateUrl: './plugin-details.component.html',
  styleUrls: ['./plugin-details.component.scss'],
  imports: [
    RouterModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    PluginDetailsHeaderComponent,
    PluginDetailsVersionsTableComponent,
    PluginDetailsBundleNavigatorComponent,
    PluginDetailsInfoComponent,
    PluginDetailsExtensionsComponent,
    ErrorStateComponent,
  ],
})
export class PluginDetailsComponent {
  private route = inject(ActivatedRoute);
  private pluginService = inject(PluginService);
  private snackbar = inject(MatSnackBar);

  projectId = signal<string | undefined>(undefined);
  pluginId = signal<string | undefined>(undefined);
  isActionLoading = signal(false);

  pluginResource = rxResource({
    params: () => ({ pluginId: this.pluginId(), projectId: this.projectId() }),
    stream: ({ params: { pluginId, projectId } }) => this.pluginService.getPlugin(pluginId!, projectId),
  });
  pluginResourceError = computed(() => convertError(this.pluginResource.error()));

  installedPlugins = rxResource({
    params: () => this.projectId(),
    stream: ({ params: projectId }) => this.pluginService.getInstalledPlugins(projectId),
  });

  installedVersion = computed(() => {
    const plugin = this.pluginResource.value();
    const installedPlugins = this.installedPlugins.value();

    if (!plugin || !installedPlugins) {
      return undefined;
    }
    const version = plugin.versions.find((v) => installedPlugins.find((ip) => ip.id === plugin.id && ip.version === v.version) !== undefined);
    return version ? version.version : undefined;
  });

  latestVersion = computed(() => {
    const plugin = this.pluginResource.value();
    if (!plugin || plugin.versions.length === 0) {
      return undefined;
    }
    return plugin.versions[plugin.versions.length - 1].version;
  });

  updateAvailable = computed(() => {
    const installed = this.installedVersion();
    const latest = this.latestVersion();
    return !!installed && !!latest && installed !== latest;
  });

  constructor() {
    let currentRoute: ActivatedRoute | null = this.route;
    while (currentRoute) {
      currentRoute.params.pipe(takeUntilDestroyed()).subscribe((params) => {
        if (params['id'] && !this.projectId()) {
          this.projectId.set(params['id']);
        }
      });
      currentRoute = currentRoute.parent;
    }

    this.route.params.pipe(takeUntilDestroyed()).subscribe((params) => {
      this.pluginId.set(params['pluginId']);
    });
  }

  onInstallButtonClick(version?: string) {
    const projectId = this.projectId();
    const plugin = this.pluginResource.value();
    const targetVersion = version || this.latestVersion();
    if (!projectId || !plugin || !targetVersion) {
      return;
    }

    this.isActionLoading.set(true);
    this.pluginService
      .installPlugin(projectId, plugin.id, targetVersion)
      .pipe(finalize(() => this.isActionLoading.set(false)))
      .subscribe({
        next: () => {
          this.pluginResource.reload();
          this.installedPlugins.reload();
          this.snackbar.open(`${plugin.type} v${targetVersion} installed successfully`, 'Dismiss', { duration: 4000 });
        },
        error: () => {
          this.snackbar.open(`Could not install ${plugin.type.toLowerCase()}`, 'Dismiss', { duration: 4000 });
        },
      });
  }

  onUninstallButtonClick() {
    const projectId = this.projectId();
    const plugin = this.pluginResource.value();
    const installedVersion = this.installedVersion();
    if (!projectId || !plugin || !installedVersion) {
      return;
    }

    this.isActionLoading.set(true);
    this.pluginService
      .uninstallPlugin(projectId, plugin.id, installedVersion)
      .pipe(finalize(() => this.isActionLoading.set(false)))
      .subscribe({
        next: () => {
          this.pluginResource.reload();
          this.installedPlugins.reload();
          this.snackbar.open(`${plugin.type} uninstalled successfully`, 'Dismiss', { duration: 4000 });
        },
        error: () => {
          this.snackbar.open(`Could not uninstall ${plugin.type.toLowerCase()}`, 'Dismiss', { duration: 4000 });
        },
      });
  }
}
