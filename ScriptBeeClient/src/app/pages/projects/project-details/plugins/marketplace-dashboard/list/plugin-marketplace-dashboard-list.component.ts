import { Component, input, output } from '@angular/core';

import { MarketplacePlugin } from '../../../../../../types/marketplace-plugin';
import { PluginMarketplaceDashboardListItemComponent } from './item/plugin-marketplace-dashboard-list-item.component';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-plugin-marketplace-dashboard-list',
  templateUrl: './plugin-marketplace-dashboard-list.component.html',
  styleUrls: ['./plugin-marketplace-dashboard-list.component.scss'],
  imports: [PluginMarketplaceDashboardListItemComponent, MatIconModule],
})
export class PluginMarketplaceDashboardListComponent {
  plugins = input<MarketplacePlugin[] | undefined>(undefined);
  projectId = input<string | undefined>(undefined);
  actionCompleted = output<void>();
}
