import { Component, Input } from '@angular/core';
import { Plugin } from "../../services/plugin/plugin";

@Component({
  selector: 'app-plugin-item',
  templateUrl: './plugin-item.component.html',
  styleUrls: ['./plugin-item.component.scss']
})
export class PluginItemComponent {

  @Input()
  plugin?: Plugin = undefined;

  getPluginAsString() {
    return JSON.stringify(this.plugin, null, 2);
  }
}
