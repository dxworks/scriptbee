import { Component, Input } from '@angular/core';
import { MarketplacePlugin } from "../../../services/plugin/marketplace-plugin";
import { PluginService } from "../../../services/plugin/plugin.service";
import { MatSnackBar } from "@angular/material/snack-bar";

@Component({
  selector: 'app-expanded-plugin-row',
  templateUrl: './expanded-plugin-row.component.html',
  styleUrls: ['./expanded-plugin-row.component.scss']
})
export class ExpandedPluginRowComponent {

  @Input() plugin: MarketplacePlugin;

  constructor(private pluginService: PluginService, private snackBar: MatSnackBar,) {
  }

  installPlugin(version: string) {
    this.pluginService.installPlugin(this.plugin.id, version).subscribe({
      next: () => {
        this.plugin.versions = this.plugin.versions.map(v => {
          if (v.version === version) {
            v.installed = true;
          }
          return v;
        });
      },
      error: () => {
        this.snackBar.open('Could not install plugin', 'Ok', {
          duration: 4000
        });
      }
    });
  }

  uninstallPlugin(version: string) {
    this.pluginService.uninstallPlugin(this.plugin.id, version).subscribe({
      next: () => {
        this.plugin.versions = this.plugin.versions.map(v => {
          if (v.version === version) {
            v.installed = false;
          }
          return v;
        });
      },
      error: () => {
        this.snackBar.open('Could not uninstall plugin', 'Ok', {
          duration: 4000
        });
      }
    });
  }
}
