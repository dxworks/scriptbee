import { Component, computed, input, output } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PluginVersion } from '../../../../../../../types/marketplace-plugin';

@Component({
  selector: 'app-plugin-details-versions-table',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatChipsModule, MatButtonModule, MatIconModule, MatTooltipModule, DatePipe],
  templateUrl: './plugin-details-versions-table.component.html',
  styleUrls: ['./plugin-details-versions-table.component.scss'],
})
export class PluginDetailsVersionsTableComponent {
  versions = input.required<PluginVersion[]>();
  installVersion = output<string>();

  sortedVersions = computed(() => {
    return [...this.versions()].sort((a, b) => new Date(b.publishDate).getTime() - new Date(a.publishDate).getTime());
  });

  displayedColumns: string[] = ['version', 'publishDate', 'status', 'actions'];
}
