import { Component, Input } from '@angular/core';
import { MarketplacePlugin } from "../../../services/plugin/marketplace-plugin";
import { PluginService } from "../../../services/plugin/plugin.service";

@Component({
  selector: 'app-expanded-plugin-row',
  templateUrl: './expanded-plugin-row.component.html',
  styleUrls: ['./expanded-plugin-row.component.scss']
})
export class ExpandedPluginRowComponent {

  @Input() plugin: MarketplacePlugin;

  constructor(private pluginService: PluginService) {
  }

  installPlugin(version: string) {
    this.pluginService.installPlugin(this.plugin.name, version).subscribe({
      next: () => {
        console.log('installed');
        // todo display success
      },
      error: () => {
        console.log('error');
        // todo display error
      }
    });
  }

  uninstallPlugin(version: string) {
    this.pluginService.uninstallPlugin(this.plugin.name, version).subscribe({
      next: () => {
        console.log('uninstalled');
        // todo display success
      },
      error: () => {
        console.log('error');
        // todo display error
      }
    });
  }

  getPluginVersions() {
    return Object.entries(this.plugin.versions);
  }
}
