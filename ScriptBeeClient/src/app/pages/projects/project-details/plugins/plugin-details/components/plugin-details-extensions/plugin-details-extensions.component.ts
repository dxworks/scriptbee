import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-plugin-details-extensions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './plugin-details-extensions.component.html',
  styleUrls: ['./plugin-details-extensions.component.scss'],
})
export class PluginDetailsExtensionsComponent {
  extensions = input.required<{ kind: string; language?: string; extension?: string }[]>();
}
