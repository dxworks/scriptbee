import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { PluginService } from '../../services/plugin/plugin.service';
import { ErrorStateComponent } from '../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../components/loading-progress-bar/loading-progress-bar.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';
import { convertError } from '../../utils/api';
import { PluginMarketplaceDashboardListComponent } from '../projects/project-details/plugins/marketplace-dashboard/list/plugin-marketplace-dashboard-list.component';

@Component({
  selector: 'app-gateway-plugins',
  imports: [
    ErrorStateComponent,
    LoadingProgressBarComponent,
    PluginMarketplaceDashboardListComponent,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatDividerModule,
    FormsModule,
  ],
  templateUrl: './gateway-plugins.component.html',
  styleUrls: ['./gateway-plugins.component.scss'],
})
export class GatewayPluginsComponent {
  private pluginsService = inject(PluginService);

  searchText = signal('');

  availablePlugins = rxResource({
    stream: () => this.pluginsService.getAllAvailablePlugins(),
  });

  gatewayPlugins = rxResource({
    stream: () => this.pluginsService.getGatewayPlugins(),
  });

  availablePluginsError = computed(() => convertError(this.availablePlugins.error()));

  filteredPlugins = computed(() => {
    const plugins = this.availablePlugins.value() ?? [];
    const search = this.searchText().toLowerCase();

    return plugins.filter((p) => p.name.toLowerCase().includes(search) || p.description.toLowerCase().includes(search));
  });

  onActionCompleted() {
    this.gatewayPlugins.reload();
  }
}
