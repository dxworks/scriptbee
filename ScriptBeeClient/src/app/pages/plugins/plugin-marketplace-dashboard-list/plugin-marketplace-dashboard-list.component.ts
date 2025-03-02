import { Component, Input } from '@angular/core';
import { MarketplaceProject } from '../../services/marketplace-project';
import { ApiErrorMessage } from '../../../shared/api-error-message';

@Component({
  selector: 'app-plugin-marketplace-dashboard-list',
  templateUrl: './plugin-marketplace-dashboard-list.component.html',
  styleUrls: ['./plugin-marketplace-dashboard-list.component.scss'],
})
export class PluginMarketplaceDashboardListComponent {
  @Input()
  plugins: MarketplaceProject[] | undefined;
  @Input()
  error: ApiErrorMessage | undefined;
}
