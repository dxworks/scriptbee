import { Component, input } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { BundleItem } from '../../../../../../../types/marketplace-plugin';

@Component({
  selector: 'app-plugin-details-bundle-navigator',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, RouterModule],
  templateUrl: './plugin-details-bundle-navigator.component.html',
  styleUrls: ['./plugin-details-bundle-navigator.component.scss'],
})
export class PluginDetailsBundleNavigatorComponent {
  items = input.required<BundleItem[]>();
}
