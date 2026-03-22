import { Component, input, output } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { RouterModule } from '@angular/router';
import { MarketplacePluginWithDetails } from '../../../../../../../types/marketplace-plugin';

@Component({
  selector: 'app-plugin-details-header',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatChipsModule, MatDividerModule, RouterModule],
  templateUrl: './plugin-details-header.component.html',
  styleUrls: ['./plugin-details-header.component.scss'],
})
export class PluginDetailsHeaderComponent {
  plugin = input.required<MarketplacePluginWithDetails>();
  installedVersion = input<string | undefined>();
  latestVersion = input<string | undefined>();
  updateAvailable = input<boolean>();

  install = output<void>();
  uninstall = output<void>();
}
