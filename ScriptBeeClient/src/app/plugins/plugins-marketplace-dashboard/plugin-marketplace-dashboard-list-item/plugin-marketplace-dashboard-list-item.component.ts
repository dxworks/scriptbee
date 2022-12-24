import { Component, Input } from '@angular/core';
import { MarketplaceProject } from '../../../services/plugin/marketplace-project';
import { PluginService } from '../../../services/plugin/plugin.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-plugin-marketplace-dashboard-list-item',
  templateUrl: './plugin-marketplace-dashboard-list-item.component.html',
  styleUrls: ['./plugin-marketplace-dashboard-list-item.component.scss']
})
export class PluginMarketplaceDashboardListItemComponent {
  @Input()
  set plugin(value: MarketplaceProject) {
    this._plugin = value;
    this.installedVersion = PluginMarketplaceDashboardListItemComponent.getInstalledVersion(value);
    this.selectedVersion = this.installedVersion;
  }

  get plugin(): MarketplaceProject {
    return this._plugin;
  }

  private _plugin: MarketplaceProject;

  installedVersion: string | undefined;
  selectedVersion: string | undefined;
  loading = false;

  constructor(private pluginService: PluginService, private snackbar: MatSnackBar) {}

  onInstallButtonClick() {
    this.loading = true;

    this.pluginService.installPlugin(this.plugin.id, this.selectedVersion).subscribe({
      next: () => {
        this.loading = false;
        this.installedVersion = this.selectedVersion;
      },
      error: () => {
        this.loading = false;
        this.snackbar.open('Could not install plugin', 'Dismiss', { duration: 4000 });
      }
    });
  }

  onUninstallButtonClick() {
    this.loading = true;

    this.pluginService.uninstallPlugin(this.plugin.id, this.installedVersion).subscribe({
      next: () => {
        this.installedVersion = undefined;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackbar.open('Could not uninstall plugin', 'Dismiss', { duration: 4000 });
      }
    });
  }

  private static getInstalledVersion(plugin: MarketplaceProject): string | undefined {
    const pluginVersion = plugin.versions.find((version) => version.installed);
    return pluginVersion ? pluginVersion.version : undefined;
  }
}
