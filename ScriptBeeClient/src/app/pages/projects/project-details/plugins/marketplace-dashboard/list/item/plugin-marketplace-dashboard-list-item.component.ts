import { Component, computed, inject, input, output, signal } from '@angular/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { RouterModule } from '@angular/router';
import { MarketplacePlugin } from '../../../../../../../types/marketplace-plugin';
import { PluginService } from '../../../../../../../services/plugin/plugin.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-plugin-marketplace-dashboard-list-item',
  templateUrl: './plugin-marketplace-dashboard-list-item.component.html',
  styleUrls: ['./plugin-marketplace-dashboard-list-item.component.scss'],
  imports: [
    MatCardModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatSnackBarModule,
    MatChipsModule,
    MatIconModule,
    MatDividerModule,
    MatTooltipModule,
    MatBadgeModule,
    RouterModule,
  ],
})
export class PluginMarketplaceDashboardListItemComponent {
  plugin = input.required<MarketplacePlugin>();
  projectId = input.required<string | undefined>();
  installedVersion = input.required<string | undefined>();

  private pluginService = inject(PluginService);
  private snackbar = inject(MatSnackBar);

  loading = signal(false);

  latestVersion = computed(() => {
    return this.plugin().versions[this.plugin().versions.length - 1]?.version;
  });

  updateAvailable = computed(() => {
    const installed = this.installedVersion();
    const latest = this.latestVersion();
    return !!installed && !!latest && installed !== latest;
  });

  actionCompleted = output<void>();

  onInstallButtonClick() {
    const projectId = this.projectId();
    const project = this.plugin();
    const versionToInstall = this.latestVersion();
    if (!versionToInstall || !project) {
      return;
    }

    this.loading.set(true);
    const request = projectId
      ? this.pluginService.installPlugin(projectId, project.id, versionToInstall)
      : this.pluginService.installGatewayPlugin(project.id, versionToInstall);

    request.pipe(finalize(() => this.loading.set(false))).subscribe({
      next: () => {
        this.actionCompleted.emit();
        this.snackbar.open(`${project.type} installed successfully`, 'Dismiss', { duration: 4000 });
      },
      error: () => {
        this.snackbar.open(`Could not install ${project.type.toLowerCase()}`, 'Dismiss', { duration: 4000 });
      },
    });
  }

  onUninstallButtonClick() {
    const projectId = this.projectId();
    const project = this.plugin();
    const installedVersion = this.installedVersion();
    if (!project || !installedVersion) {
      return;
    }

    this.loading.set(true);
    const request = projectId
      ? this.pluginService.uninstallPlugin(projectId, project.id, installedVersion)
      : this.pluginService.uninstallGatewayPlugin(project.id, installedVersion);

    request.pipe(finalize(() => this.loading.set(false))).subscribe({
      next: () => {
        this.actionCompleted.emit();
        this.snackbar.open(`${project.type} uninstalled successfully`, 'Dismiss', { duration: 4000 });
      },
      error: () => {
        this.snackbar.open(`Could not uninstall ${project.type.toLowerCase()}`, 'Dismiss', { duration: 4000 });
      },
    });
  }
}
