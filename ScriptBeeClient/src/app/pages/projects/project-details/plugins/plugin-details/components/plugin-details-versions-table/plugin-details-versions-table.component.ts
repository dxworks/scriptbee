import { Component, input, output } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PluginVersion } from '../../../../../../../types/marketplace-plugin';

@Component({
  selector: 'app-plugin-details-versions-table',
  standalone: true,
  imports: [MatTableModule, MatChipsModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './plugin-details-versions-table.component.html',
  styleUrls: ['./plugin-details-versions-table.component.scss'],
})
export class PluginDetailsVersionsTableComponent {
  versions = input.required<PluginVersion[]>();
  installedVersion = input<string | undefined>();
  installVersion = output<string>();

  displayedColumns: string[] = ['version', 'status', 'actions'];
}
