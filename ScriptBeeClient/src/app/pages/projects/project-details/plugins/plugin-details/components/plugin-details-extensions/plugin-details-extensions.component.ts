import { Component, input } from '@angular/core';

@Component({
  selector: 'app-plugin-details-extensions',
  standalone: true,
  imports: [],
  templateUrl: './plugin-details-extensions.component.html',
  styleUrls: ['./plugin-details-extensions.component.scss'],
})
export class PluginDetailsExtensionsComponent {
  extensions = input.required<{ kind: string; language?: string; extension?: string }[]>();
}
