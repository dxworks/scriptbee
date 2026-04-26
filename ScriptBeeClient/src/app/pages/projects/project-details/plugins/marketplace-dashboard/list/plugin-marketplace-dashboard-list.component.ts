import { Component, input, output } from '@angular/core';

import { InstalledPlugin, MarketplacePlugin } from '../../../../../../types/marketplace-plugin';
import { PluginMarketplaceDashboardListItemComponent } from './item/plugin-marketplace-dashboard-list-item.component';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-plugin-marketplace-dashboard-list',
  templateUrl: './plugin-marketplace-dashboard-list.component.html',
  styleUrls: ['./plugin-marketplace-dashboard-list.component.scss'],
  imports: [PluginMarketplaceDashboardListItemComponent, MatIconModule],
})
export class PluginMarketplaceDashboardListComponent {
  plugins = input.required<MarketplacePlugin[]>();
  installedPlugins = input.required<InstalledPlugin[]>();
  projectId = input<string | undefined>();
  actionCompleted = output<void>();

  getInstalledVersion(marketPlacePlugin: MarketplacePlugin): string | undefined {
    const installedPlugin = this.installedPlugins().find((ip) => ip.id === marketPlacePlugin.id);
    if (!installedPlugin) {
      return undefined;
    }
    const version = marketPlacePlugin.versions.find((v) => v.version === installedPlugin.version);
    return version ? version.version : undefined;
  }
}
