import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GatewayPluginsComponent } from './gateway-plugins.component';
import { PluginService } from '../../services/plugin/plugin.service';
import { beforeEach, describe, expect, it, Mock, vi } from 'vitest';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { InstalledPlugin, MarketplacePlugin } from '../../types/marketplace-plugin';

describe('GatewayPluginsComponent', () => {
  let component: GatewayPluginsComponent;
  let fixture: ComponentFixture<GatewayPluginsComponent>;

  let pluginServiceSpy: {
    getAllAvailablePlugins: Mock;
    getGatewayPlugins: Mock;
    installGatewayPlugin: Mock;
    uninstallGatewayPlugin: Mock;
  };

  const mockAvailablePlugins: MarketplacePlugin[] = [
    {
      id: 'Plugin.A',
      name: 'Plugin A',
      type: 'Plugin',
      description: 'First plugin',
      authors: ['Author 1'],
      versions: [{ version: '1.0.0', url: false, manifestUrl: '' }],
    },
    {
      id: 'Plugin.B',
      name: 'Plugin B',
      type: 'Bundle',
      description: 'Second plugin',
      authors: ['Author 2'],
      versions: [{ version: '2.0.0', url: false, manifestUrl: '' }],
    },
  ] as MarketplacePlugin[];

  const mockGatewayPlugins: InstalledPlugin[] = [{ id: 'Plugin.A', version: '1.0.0' }];

  beforeEach(async () => {
    pluginServiceSpy = {
      getAllAvailablePlugins: vi.fn().mockReturnValue(of(mockAvailablePlugins)),
      getGatewayPlugins: vi.fn().mockReturnValue(of(mockGatewayPlugins)),
      installGatewayPlugin: vi.fn().mockReturnValue(of(undefined)),
      uninstallGatewayPlugin: vi.fn().mockReturnValue(of(undefined)),
    };

    await TestBed.configureTestingModule({
      imports: [GatewayPluginsComponent],
      providers: [{ provide: PluginService, useValue: pluginServiceSpy }, provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(GatewayPluginsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  });

  describe('Business Perspective: Marketplace Discovery', () => {
    it('should display all available plugins from the marketplace initially', () => {
      const cards = fixture.debugElement.queryAll(By.css('mat-card'));
      expect(cards.length).toBe(mockAvailablePlugins.length);
      expect(fixture.nativeElement.textContent).toContain('Plugin A');
      expect(fixture.nativeElement.textContent).toContain('Plugin B');
    });

    it('should filter plugins when user types in the search bar', async () => {
      component.searchText.set('Second');
      fixture.detectChanges();
      await fixture.whenStable();

      const cards = fixture.debugElement.queryAll(By.css('mat-card'));
      expect(cards.length).toBe(1);
      expect(fixture.nativeElement.textContent).toContain('Plugin B');
      expect(fixture.nativeElement.textContent).not.toContain('Plugin A');
    });

    it('should identify which plugins are already installed in the Gateway', () => {
      const installedBadge = fixture.debugElement.query(By.css('.installed-v'));
      expect(installedBadge.nativeElement.textContent).toContain('v1.0.0');
    });
  });

  describe('Business Perspective: Action Feedback', () => {
    it('should update the displayed list of gateway plugins when an action (install/uninstall) is completed', async () => {
      const updatedGatewayPlugins = [...mockGatewayPlugins, { id: 'Plugin.B', version: '2.0.0' }];
      pluginServiceSpy.getGatewayPlugins.mockReturnValue(of(updatedGatewayPlugins));

      component.onActionCompleted();
      fixture.detectChanges();
      await fixture.whenStable();

      const installedVersions = fixture.debugElement.queryAll(By.css('.installed-v'));
      expect(installedVersions.length).toBe(2);
      expect(installedVersions[0].nativeElement.textContent).toContain('v1.0.0');
      expect(installedVersions[1].nativeElement.textContent).toContain('v2.0.0');
    });
  });
});
