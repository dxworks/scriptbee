import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { PluginService } from '../../../../../services/plugin/plugin.service';
import { MarketplacePluginWithDetails, PluginVersion } from '../../../../../types/marketplace-plugin';
import { PluginDetailsHeaderComponent } from './components/plugin-details-header/plugin-details-header.component';
import { PluginDetailsVersionsTableComponent } from './components/plugin-details-versions-table/plugin-details-versions-table.component';
import { PluginDetailsBundleNavigatorComponent } from './components/plugin-details-bundle-navigator/plugin-details-bundle-navigator.component';
import { PluginDetailsInfoComponent } from './components/plugin-details-info/plugin-details-info.component';
import { PluginDetailsExtensionsComponent } from './components/plugin-details-extensions/plugin-details-extensions.component';

@Component({
  selector: 'app-plugin-details',
  templateUrl: './plugin-details.component.html',
  styleUrls: ['./plugin-details.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
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
  ],
})
export class PluginDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private pluginService = inject(PluginService);
  private snackbar = inject(MatSnackBar);

  projectId = signal<string | undefined>(undefined);
  pluginId = signal<string | undefined>(undefined);
  plugin = signal<MarketplacePluginWithDetails | undefined>(undefined);
  loading = signal(false);

  installedVersion = computed(() => {
    const p = this.plugin();
    if (!p) {
      return undefined;
    }
    const version = p.versions.find((v: PluginVersion) => v.installed);
    return version ? version.version : undefined;
  });

  latestVersion = computed(() => {
    const p = this.plugin();
    if (!p || p.versions.length === 0) {
      return undefined;
    }
    return p.versions[p.versions.length - 1].version;
  });

  updateAvailable = computed(() => {
    const installed = this.installedVersion();
    const latest = this.latestVersion();
    return !!installed && !!latest && installed !== latest;
  });

  ngOnInit() {
    let currentRoute: ActivatedRoute | null = this.route;
    while (currentRoute) {
      currentRoute.params.subscribe((params) => {
        if (params['id'] && !this.projectId()) {
          this.projectId.set(params['id']);
          this.loadPlugin();
        }
      });
      currentRoute = currentRoute.parent;
    }

    this.route.params.subscribe((params) => {
      this.pluginId.set(params['pluginId']);
      this.loadPlugin();
    });
  }

  loadPlugin() {
    const pId = this.pluginId();
    if (!pId) {
      return;
    }

    this.loading.set(true);
    this.pluginService.getPlugin(pId).subscribe({
      next: (plugin: MarketplacePluginWithDetails) => {
        this.plugin.set(plugin);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.open('Could not load plugin details', 'Dismiss', { duration: 4000 });
      },
    });
  }

  onInstallButtonClick(version?: string) {
    const projId = this.projectId();
    const p = this.plugin();
    const targetVersion = version || this.latestVersion();
    if (!projId || !p || !targetVersion) {
      return;
    }

    this.loading.set(true);
    this.pluginService.installPlugin(projId, p.id, targetVersion).subscribe({
      next: () => {
        this.loadPlugin();
        this.snackbar.open(`${p.type} v${targetVersion} installed successfully`, 'Dismiss', { duration: 4000 });
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.open(`Could not install ${p.type.toLowerCase()}`, 'Dismiss', { duration: 4000 });
      },
    });
  }

  onInstallSpecificVersion(version: string) {
    this.onInstallButtonClick(version);
  }

  onUninstallButtonClick() {
    const projId = this.projectId();
    const p = this.plugin();
    if (!projId || !p) {
      return;
    }

    this.loading.set(true);
    this.pluginService.uninstallPlugin(projId, p.id).subscribe({
      next: () => {
        this.loadPlugin();
        this.snackbar.open(`${p.type} uninstalled successfully`, 'Dismiss', { duration: 4000 });
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.open(`Could not uninstall ${p.type.toLowerCase()}`, 'Dismiss', { duration: 4000 });
      },
    });
  }
}
