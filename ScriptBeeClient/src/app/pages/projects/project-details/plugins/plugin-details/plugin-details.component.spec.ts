import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PluginDetailsComponent } from './plugin-details.component';
import { PluginService } from '../../../../../services/plugin/plugin.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { beforeEach, describe, expect, it, Mock, vi } from 'vitest';
import { By } from '@angular/platform-browser';
import { HttpErrorResponse } from '@angular/common/http';

describe('PluginDetailsComponent', () => {
  let fixture: ComponentFixture<PluginDetailsComponent>;

  let pluginServiceSpy: {
    getPlugin: Mock;
    getInstalledPlugins: Mock;
  };

  beforeEach(async () => {
    pluginServiceSpy = {
      getPlugin: vi.fn(),
      getInstalledPlugins: vi.fn().mockReturnValue(of([])),
    };

    const activatedRouteMock = {
      params: of({ pluginId: 'test-plugin' }),
      parent: {
        params: of({ id: 'project-id' }),
        parent: null,
      },
    };

    await TestBed.configureTestingModule({
      imports: [PluginDetailsComponent],
      providers: [
        { provide: PluginService, useValue: pluginServiceSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PluginDetailsComponent);
  });

  it('should display error state when plugin is not found', async () => {
    // Arrange
    const errorResponse = new HttpErrorResponse({
      error: { title: 'Plugin Not Found' },
      status: 404,
    });
    pluginServiceSpy.getPlugin.mockReturnValue(throwError(() => errorResponse));

    // Act
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    // Assert
    const errorState = fixture.debugElement.query(By.css('app-error-state'));
    expect(errorState).toBeTruthy();
    expect(errorState.nativeElement.textContent).toContain('Plugin Not Found');
  });

  it('should display plugin details when plugin exists', async () => {
    // Arrange
    const mockPlugin = {
      id: 'test-plugin',
      name: 'Test Plugin',
      type: 'Plugin',
      authors: [],
      versions: [{ version: '1.0.0' }],
    };
    pluginServiceSpy.getPlugin.mockReturnValue(of(mockPlugin));

    // Act
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    // Assert
    const header = fixture.debugElement.query(By.css('app-plugin-details-header'));
    expect(header).toBeTruthy();
    expect(fixture.debugElement.query(By.css('app-error-state'))).toBeFalsy();
  });
});
