import { MatListModule } from '@angular/material/list';
import { PluginsMarketplaceDashboardComponent } from './plugins-marketplace-dashboard.component';
import { createComponentFactory, createHttpFactory, HttpMethod, Spectator, SpectatorHttp } from '@ngneat/spectator';
import { PluginService } from '../../services/plugin.service';
import { MockComponents, MockDirectives } from 'ng-mocks';
import { PluginMarketplaceDashboardListComponent } from '../plugin-marketplace-dashboard-list/plugin-marketplace-dashboard-list.component';
import { By } from '@angular/platform-browser';
import { MarketplaceProject } from '../../services/marketplace-project';
import { createMarketplacePlugin } from '../../../../../test/marketplacePluginUtils';
import { MatFormField, MatLabel } from '@angular/material/form-field';

describe('PluginsMarketplaceDashboardComponent', () => {
  let pluginServiceSpectator: SpectatorHttp<PluginService>;
  const createPluginService = createHttpFactory({ service: PluginService });

  const createComponent = createComponentFactory({
    component: PluginsMarketplaceDashboardComponent,
    declarations: [...MockComponents(PluginMarketplaceDashboardListComponent, MatFormField), ...MockDirectives(MatLabel)],
    imports: [MatListModule],
  });

  function mockGetAvailablePluginsApi(
    component: Spectator<PluginsMarketplaceDashboardComponent>,
    plugins: MarketplaceProject[],
    options?: { status: number; statusText: string }
  ) {
    pluginServiceSpectator.expectOne('/api/plugins/available', HttpMethod.GET).flush(plugins, options);

    component.fixture.detectChanges();
  }

  beforeEach(() => {
    pluginServiceSpectator = createPluginService();
  });

  it('should create', () => {
    const component = createComponent();

    mockGetAvailablePluginsApi(component, []);

    expect(component).toBeTruthy();
  });

  it('should have categories', () => {
    const categories = ['Bundle', 'Loader', 'Linker', 'Script Runner', 'Script Generator', 'Helper Functions'];
    const component = createComponent();

    mockGetAvailablePluginsApi(component, []);

    categories.forEach((category) => {
      const categoryElement = component.debugElement.query((e) => e.nativeElement.textContent === category);
      expect(categoryElement).toBeDefined();
    });
  });

  it('should have plugins', () => {
    const component = createComponent();
    mockGetAvailablePluginsApi(component, [
      createMarketplacePlugin('1', 'plugin-name1'),
      createMarketplacePlugin('2', 'plugin-name2'),
      createMarketplacePlugin('3', 'plugin-name3'),
    ]);

    const pluginMarketplaceDashboardListComponent = component.debugElement.query(By.directive(PluginMarketplaceDashboardListComponent));

    expect(pluginMarketplaceDashboardListComponent.componentInstance.plugins).toHaveLength(3);
  });

  it('should have error', () => {
    const component = createComponent();
    mockGetAvailablePluginsApi(component, [], { status: 400, statusText: 'error' });

    const pluginMarketplaceDashboardListComponent = component.debugElement.query(By.directive(PluginMarketplaceDashboardListComponent));

    expect(pluginMarketplaceDashboardListComponent.componentInstance.error).toBeDefined();
  });
});
