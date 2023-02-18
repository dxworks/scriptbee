import { PluginMarketplaceDashboardListItemComponent } from './plugin-marketplace-dashboard-list-item.component';
import { createComponentFactory, createHttpFactory, HttpMethod, Spectator, SpectatorHttp } from '@ngneat/spectator';
import { createMarketplacePlugin, createPluginVersion } from '../../../../../test/marketplacePluginUtils';
import { MockComponents, MockDirectives } from 'ng-mocks';
import { MatCard, MatCardActions, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PluginService } from '../../services/plugin.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSelectHarness } from '@angular/material/select/testing';
import { expect } from '@angular/flex-layout/_private-utils/testing';

describe('PluginMarketplaceDashboardListItemComponent', () => {
  let pluginServiceSpectator: SpectatorHttp<PluginService>;
  const createPluginService = createHttpFactory({ service: PluginService });

  const createComponent = createComponentFactory({
    component: PluginMarketplaceDashboardListItemComponent,
    declarations: [...MockComponents(MatCard, MatCardHeader), ...MockDirectives(MatCardTitle, MatCardSubtitle, MatCardActions, MatCardContent)],
    imports: [MatSelectModule, MatProgressSpinnerModule, MatSnackBarModule],
  });

  beforeEach(() => {
    pluginServiceSpectator = createPluginService();
  });

  describe('basic information', () => {
    it('should create', () => {
      const component = createComponent({ props: { plugin: createMarketplacePlugin('1', '2') } });
      expect(component).toBeTruthy();
    });

    it('given plugin, then plugin name is shown', () => {
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin') } });

      const element = component.debugElement.query((e) => e.nativeElement.textContent === 'plugin-name');

      expect(element).toBeTruthy();
    });

    it('given plugin, then plugin type is shown', () => {
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Bundle') } });

      const element = component.debugElement.query((e) => e.nativeElement.textContent === 'Bundle');

      expect(element).toBeTruthy();
    });

    it('given plugin, when version array is empty, then no version text is shown', () => {
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin') } });

      const element = component.debugElement.query((e) => e.nativeElement.textContent === 'No version is installed');

      expect(element).toBeTruthy();
    });
  });

  describe('installed plugin version', () => {
    function getUninstallButton(component: Spectator<PluginMarketplaceDashboardListItemComponent>) {
      return component.debugElement.nativeElement.querySelector('button');
    }

    function clickUninstallButton(
      component: Spectator<PluginMarketplaceDashboardListItemComponent>,
      id: string,
      version: string,
      options?: { status: number; statusText: string }
    ) {
      const uninstallButton = getUninstallButton(component);

      uninstallButton.click();
      pluginServiceSpectator.expectOne(`/api/plugins/uninstall/${id}/${version}`, HttpMethod.DELETE).flush({}, options);
      component.fixture.detectChanges();
    }

    it('given plugin, then installed version text is shown', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });

      const element = component.debugElement.query((e) => e.nativeElement.textContent === 'Installed version: 1.0.0');

      expect(element).toBeTruthy();
    });

    it('given plugin, then uninstall button is show', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });
      const uninstallButton = component.debugElement.nativeElement.querySelector('button');

      expect(uninstallButton.textContent).toContain('UNINSTALL');
    });

    it('given uninstall button click, then loading wheel is shown until api response is present', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeFalsy();

      getUninstallButton(component).click();
      const req = pluginServiceSpectator.expectOne(`/api/plugins/uninstall/id/1.0.0`, HttpMethod.DELETE);
      component.fixture.detectChanges();

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeTruthy();
      req.flush({});
    });

    it('given uninstall button click, then button is disabled until api response is present', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });

      getUninstallButton(component).click();
      const req = pluginServiceSpectator.expectOne(`/api/plugins/uninstall/id/1.0.0`, HttpMethod.DELETE);
      component.fixture.detectChanges();

      expect(getUninstallButton(component).disabled).toBeTruthy();
      req.flush({});
    });

    it('given uninstall button click, when uninstall is successful, then loading wheel disappears', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      clickUninstallButton(component, 'plugin-id', '1.0.0');

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeFalsy();
    });

    it('given uninstall button click, when uninstall is failed, then loading wheel disappears', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      clickUninstallButton(component, 'plugin-id', '1.0.0', { status: 500, statusText: 'Internal Server Error' });

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeFalsy();
    });

    it('given uninstall button click, when uninstall is failed, then error is displayed to user', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });
      const snackBarOpenSpy = spyOn(component.inject(MatSnackBar), 'open');

      clickUninstallButton(component, 'plugin-id', '1.0.0', { status: 500, statusText: 'Internal Server Error' });

      expect(snackBarOpenSpy).toHaveBeenCalledWith('Could not uninstall plugin', 'Dismiss', { duration: 4000 });
    });

    it('given uninstall button click, when uninstall is successful, then install button is visible', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      clickUninstallButton(component, 'plugin-id', '1.0.0');

      expect(getUninstallButton(component).textContent).not.toContain('UNINSTALL');
    });

    it('given uninstall button click, when uninstall is failed, then uninstall button is still visible', () => {
      const version = createPluginVersion(true, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      clickUninstallButton(component, 'plugin-id', '1.0.0', { status: 500, statusText: 'Internal Server Error' });

      expect(getUninstallButton(component).textContent).toContain('UNINSTALL');
    });
  });

  describe('no plugin version installed', () => {
    async function selectItemInSelect(spectator: Spectator<PluginMarketplaceDashboardListItemComponent>, index = 0) {
      const loader = TestbedHarnessEnvironment.loader(spectator.fixture);

      const select = await loader.getHarness(MatSelectHarness);
      await select.open();

      const options = await select.getOptions();
      await options[index].click();

      spectator.fixture.detectChanges();
    }

    function getInstallButton(component: Spectator<PluginMarketplaceDashboardListItemComponent>) {
      return component.debugElement.nativeElement.querySelector('button');
    }

    function clickInstallButton(
      component: Spectator<PluginMarketplaceDashboardListItemComponent>,
      id: string,
      version: string,
      options?: { status: number; statusText: string }
    ) {
      const uninstallButton = getInstallButton(component);

      uninstallButton.click();
      const req = pluginServiceSpectator.expectOne('/api/plugins/install', HttpMethod.POST);
      req.flush({}, options);
      component.fixture.detectChanges();

      expect(req.request.body).toEqual({ pluginId: id, version: version });
    }

    it('given plugin, then no version text is shown', () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });

      const element = component.debugElement.query((e) => e.nativeElement.textContent === 'No version is installed');

      expect(element).toBeTruthy();
    });

    it('given plugin, then available version select is present', () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });

      const select = component.debugElement.nativeElement.querySelector('mat-select');

      expect(select).toBeTruthy();
      expect(component.debugElement.query((e) => e.nativeElement.textContent === 'Available versions')).toBeTruthy();
    });

    it('given plugin, then available version options are present', async () => {
      const version1 = createPluginVersion(false, '1.0.0');
      const version2 = createPluginVersion(false, '1.2.0');
      const version3 = createPluginVersion(false, '4.0.0');
      const component = createComponent({
        props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version1, version2, version3]) },
      });
      const loader = TestbedHarnessEnvironment.loader(component.fixture);

      const select = await loader.getHarness(MatSelectHarness);
      await select.open();
      const options = await select.getOptions();

      expect(options.length).toBe(3);
      expect(await options[0].getText()).toBe('1.0.0');
      expect(await options[1].getText()).toBe('1.2.0');
      expect(await options[2].getText()).toBe('4.0.0');
    });

    it('given plugin, then install button is shown', () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });
      const installButton = component.debugElement.nativeElement.querySelector('button');

      expect(installButton.textContent).toContain('INSTALL');
    });

    it('given plugin, when no version is selected, then install button is disabled', () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version]) } });

      const installButton = component.debugElement.nativeElement.querySelector('button');

      expect(installButton.disabled).toBeTruthy();
    });

    it('given plugin, when a version is selected, then install button is enabled', async () => {
      const version1 = createPluginVersion(false, '1.0.0');
      const version2 = createPluginVersion(false, '6.0.0');
      const version3 = createPluginVersion(false, '1.7.0');
      const component = createComponent({
        props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version1, version2, version3]) },
      });

      await selectItemInSelect(component, 1);

      expect(getInstallButton(component).disabled).toBeFalsy();
    });

    it('given install button click, then loading wheel is shown', async () => {
      const version1 = createPluginVersion(false, '1.0.0');
      const version2 = createPluginVersion(false, '6.0.0');
      const version3 = createPluginVersion(false, '1.7.0');
      const component = createComponent({
        props: { plugin: createMarketplacePlugin('id', 'plugin-name', 'Plugin', [version1, version2, version3]) },
      });

      await selectItemInSelect(component, 1);

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeFalsy();
      component.debugElement.nativeElement.querySelector('button').click();
      component.fixture.detectChanges();

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeTruthy();
      const req = pluginServiceSpectator.expectOne('/api/plugins/install', HttpMethod.POST);
      req.flush({});
      expect(req.request.body).toEqual({ pluginId: 'id', version: '6.0.0' });
    });

    it('given install button click, when install is successful, then loading wheel disappears', async () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      await selectItemInSelect(component, 0);
      clickInstallButton(component, 'plugin-id', '1.0.0');

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeFalsy();
    });

    it('given install button click, when install is failed, then loading wheel disappears', async () => {
      const version = createPluginVersion(false, '2.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      await selectItemInSelect(component, 0);
      clickInstallButton(component, 'plugin-id', '2.0.0', { status: 500, statusText: 'Internal Server Error' });

      expect(component.debugElement.nativeElement.querySelector('mat-spinner')).toBeFalsy();
    });

    it('given install button click, when install is failed, then error is displayed to user', async () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });
      const snackBarOpenSpy = spyOn(component.inject(MatSnackBar), 'open');

      await selectItemInSelect(component, 0);
      clickInstallButton(component, 'plugin-id', '1.0.0', { status: 500, statusText: 'Internal Server Error' });

      expect(snackBarOpenSpy).toHaveBeenCalledWith('Could not install plugin', 'Dismiss', { duration: 4000 });
    });

    it('given install button click, when install is successful, then uninstall button is visible', async () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      await selectItemInSelect(component, 0);
      clickInstallButton(component, 'plugin-id', '1.0.0');

      expect(getInstallButton(component).textContent).toContain('UNINSTALL');
    });

    it('given install button click, when install is failed, then install button is still visible', async () => {
      const version = createPluginVersion(false, '1.0.0');
      const component = createComponent({ props: { plugin: createMarketplacePlugin('plugin-id', 'plugin-name', 'Plugin', [version]) } });

      await selectItemInSelect(component, 0);
      clickInstallButton(component, 'plugin-id', '1.0.0', { status: 500, statusText: 'Internal Server Error' });

      expect(getInstallButton(component).textContent).not.toContain('UNINSTALL');
    });
  });
});
