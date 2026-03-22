import { Component, input } from '@angular/core';
import { MarketplacePluginWithDetails } from '../../../../../../../types/marketplace-plugin';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-plugin-details-info',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatChipsModule],
  templateUrl: './plugin-details-info.component.html',
  styleUrls: ['./plugin-details-info.component.scss'],
})
export class PluginDetailsInfoComponent {
  plugin = input.required<MarketplacePluginWithDetails>();
}
