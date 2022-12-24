import { Component, OnInit } from '@angular/core';
import { PluginsStore } from '../plugins.store';

@Component({
  selector: 'app-plugins-marketplace-dashboard',
  templateUrl: './plugins-marketplace-dashboard.component.html',
  styleUrls: ['./plugins-marketplace-dashboard.component.scss'],
  providers: [PluginsStore]
})
export class PluginsMarketplaceDashboardComponent implements OnInit {
  marketPlacePlugins$ = this.store.marketPlacePlugins;
  marketplacePluginsError$ = this.store.marketplacePluginsError;

  constructor(private store: PluginsStore) {}

  ngOnInit(): void {
    this.store.loadMarketplacePlugins();
  }
}
