import { Component, Input } from '@angular/core';
import { MarketplaceSinglePlugin } from "../../../services/plugin/marketplace-plugin";

@Component({
  selector: 'app-expanded-plugin-row',
  templateUrl: './expanded-plugin-row.component.html',
  styleUrls: ['./expanded-plugin-row.component.scss']
})
export class ExpandedPluginRowComponent {

  @Input() plugin: MarketplaceSinglePlugin;

  constructor() {
  }

  uninstallPlugin(version: string) {

  }

  installPlugin(version: string) {

  }

  getPluginVersions() {
    return Object.entries(this.plugin.versions);
  }
}
