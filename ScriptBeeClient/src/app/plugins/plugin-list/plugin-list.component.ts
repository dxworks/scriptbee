import { Component, OnInit } from '@angular/core';
import { PluginService } from "../../services/plugin/plugin.service";
import { Plugin } from "../../services/plugin/plugin";

@Component({
  selector: 'app-plugin-list',
  templateUrl: './plugin-list.component.html',
  styleUrls: ['./plugin-list.component.scss']
})
export class PluginListComponent implements OnInit {

  plugins: Plugin[] = [];
  loading = true;

  constructor(private pluginService: PluginService) {
  }

  ngOnInit(): void {
    this.loading = true;
    this.pluginService.getAllLoadedPlugins().subscribe(plugins => {
      this.loading = false;
      this.plugins = plugins;
    })
  }
}
