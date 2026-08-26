import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { signal } from '@angular/core';
import { permissionGuard } from './permissions.guard';
import { PermissionsService } from '../services/auth/permissions.service';

describe('permissionGuard', () => {
  let permissionsServiceMock: {
    hasPermission: ReturnType<typeof vi.fn>;
    globalStatus: ReturnType<typeof signal<'idle' | 'loading' | 'reloading' | 'resolved' | 'error'>>;
  };
  let routerMock: {
    createUrlTree: ReturnType<typeof vi.fn>;
  };

  beforeEach(() => {
    permissionsServiceMock = {
      hasPermission: vi.fn(),
      globalStatus: signal<'idle' | 'loading' | 'reloading' | 'resolved' | 'error'>('resolved'),
    };

    routerMock = {
      createUrlTree: vi.fn().mockReturnValue({} as UrlTree),
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: PermissionsService, useValue: permissionsServiceMock },
        { provide: Router, useValue: routerMock },
      ],
    });
  });

  it('should allow navigation when user has permission', () => {
    // Arrange
    permissionsServiceMock.hasPermission.mockReturnValue(true);
    const guard = permissionGuard('gateway_plugin:management');

    // Act
    const result = TestBed.runInInjectionContext(() => guard({} as never, {} as never));

    // Assert
    expect(result).toBe(true);
  });

  it('should redirect to /projects when user does not have permission', () => {
    // Arrange
    permissionsServiceMock.hasPermission.mockReturnValue(false);
    const expectedUrlTree = {} as UrlTree;
    routerMock.createUrlTree.mockReturnValue(expectedUrlTree);
    const guard = permissionGuard('gateway_plugin:management');

    // Act
    const result = TestBed.runInInjectionContext(() => guard({} as never, {} as never));

    // Assert
    expect(routerMock.createUrlTree).toHaveBeenCalledWith(['/projects']);
    expect(result).toBe(expectedUrlTree);
  });
});
