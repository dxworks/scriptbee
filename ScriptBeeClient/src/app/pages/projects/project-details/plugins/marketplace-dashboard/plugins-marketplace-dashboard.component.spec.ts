import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PluginsMarketplaceDashboardComponent } from './plugins-marketplace-dashboard.component';
import { PluginService } from '../../../../../services/plugin/plugin.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { beforeEach, describe, expect, it, Mock, vi } from 'vitest';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { InstalledPlugin } from '../../../../../types/marketplace-plugin';

describe('PluginsMarketplaceDashboardComponent', () => {
  let component: PluginsMarketplaceDashboardComponent;
  let fixture: ComponentFixture<PluginsMarketplaceDashboardComponent>;

  let pluginServiceSpy: {
    getAllAvailablePlugins: Mock;
    getInstalledPlugins: Mock;
    uploadPlugin: Mock;
    uninstallPlugin: Mock;
  };
  let snackBarSpy: { open: Mock };

  beforeEach(async () => {
    pluginServiceSpy = {
      getAllAvailablePlugins: vi.fn().mockReturnValue(of([])),
      getInstalledPlugins: vi.fn().mockReturnValue(of([])),
      uploadPlugin: vi.fn().mockReturnValue(of(undefined)),
      uninstallPlugin: vi.fn().mockReturnValue(of(undefined)),
    };

    snackBarSpy = { open: vi.fn() };

    const mockParamMap = new Map<string, string>();
    mockParamMap.set('id', 'test-project-id');

    const activatedRouteMock = {
      paramMap: of({
        has: (key: string) => mockParamMap.has(key),
        get: (key: string) => mockParamMap.get(key) || null,
        getAll: (key: string) => [mockParamMap.get(key) || ''],
        keys: Array.from(mockParamMap.keys()),
      } as ParamMap),
      parent: null,
    };

    await TestBed.configureTestingModule({
      imports: [PluginsMarketplaceDashboardComponent],
    })
      .overrideComponent(PluginsMarketplaceDashboardComponent, {
        add: {
          providers: [
            { provide: PluginService, useValue: pluginServiceSpy },
            { provide: MatSnackBar, useValue: snackBarSpy },
            { provide: ActivatedRoute, useValue: activatedRouteMock },
          ],
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(PluginsMarketplaceDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  });

  describe('Manual Plugin Upload and Management', () => {
    it('should upload plugin through the file input and show success notification', async () => {
      const fileInput = fixture.debugElement.query(By.css('input[type="file"]')).nativeElement as HTMLInputElement;
      const file = new File(['mock content'], 'test-plugin.zip', { type: 'application/zip' });

      const dataTransfer = new DataTransfer();
      dataTransfer.items.add(file);
      fileInput.files = dataTransfer.files;

      fileInput.dispatchEvent(new Event('change'));
      fixture.detectChanges();
      await fixture.whenStable();
      await new Promise((resolve) => setTimeout(resolve, 100));
      fixture.detectChanges();

      expect(pluginServiceSpy.uploadPlugin).toHaveBeenCalledWith('test-project-id', file);
      expect(snackBarSpy.open).toHaveBeenCalledWith('Plugin uploaded successfully', 'Close', { duration: 3000 });
      expect(fileInput.value).toBe('');
    });

    it('should show error notification when upload fails', async () => {
      const errorMessage = 'Invalid zip format';
      pluginServiceSpy.uploadPlugin.mockReturnValueOnce(throwError(() => ({ error: { detail: errorMessage } })));

      const fileInput = fixture.debugElement.query(By.css('input[type="file"]')).nativeElement as HTMLInputElement;
      const file = new File(['mock content'], 'test-plugin.zip', { type: 'application/zip' });

      const dataTransfer = new DataTransfer();
      dataTransfer.items.add(file);
      fileInput.files = dataTransfer.files;

      fileInput.dispatchEvent(new Event('change'));
      fixture.detectChanges();
      await fixture.whenStable();
      await new Promise((resolve) => setTimeout(resolve, 100));
      fixture.detectChanges();

      expect(pluginServiceSpy.uploadPlugin).toHaveBeenCalledWith('test-project-id', file);
      expect(snackBarSpy.open).toHaveBeenCalledWith(errorMessage, 'Close', { duration: 5000 });
      expect(fileInput.value).toBe('');
    });

    it('should display manually installed plugins in the list with a distinctive icon', async () => {
      // Arrange
      const customPlugin: InstalledPlugin = { id: 'Custom.Plugin', version: '1.0.0' };
      pluginServiceSpy.getInstalledPlugins.mockReturnValue(of([customPlugin]));
      pluginServiceSpy.getAllAvailablePlugins.mockReturnValue(of([]));

      component.onActionCompleted();
      fixture.detectChanges();
      await fixture.whenStable();
      fixture.detectChanges();

      // Assert
      const titles = fixture.debugElement.queryAll(By.css('mat-card-title'));
      const customPluginTitle = titles.find((t) => t.nativeElement.textContent.includes('Custom.Plugin'));
      expect(customPluginTitle).toBeTruthy();

      // Assert
      const card = fixture.debugElement.query(By.css('app-plugin-marketplace-dashboard-list-item'));
      const icon = card.query(By.css('mat-icon')).nativeElement as HTMLElement;
      expect(icon.textContent?.trim()).toContain('cloud_upload');
    });

    it('should allow uninstalling a manually installed plugin', async () => {
      // Arrange
      const customPlugin: InstalledPlugin = { id: 'Custom.Plugin', version: '1.0.0' };
      pluginServiceSpy.getInstalledPlugins.mockReturnValue(of([customPlugin]));
      component.onActionCompleted();
      fixture.detectChanges();
      await fixture.whenStable();
      fixture.detectChanges();

      // Act
      const uninstallButton = fixture.debugElement.query(By.css('button[color="warn"]')).nativeElement as HTMLButtonElement;
      expect(uninstallButton.textContent).toContain('UNINSTALL');
      uninstallButton.click();
      fixture.detectChanges();

      // Assert
      expect(pluginServiceSpy.uninstallPlugin).toHaveBeenCalledWith('test-project-id', 'Custom.Plugin', '1.0.0');
    });
  });
});
