import { TestBed } from '@angular/core/testing';
import { HttpClient } from '@angular/common/http';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { BehaviorSubject, of } from 'rxjs';
import { PermissionsService } from './permissions.service';
import { ProjectService } from '../projects/project.service';
import { AuthService } from './AuthService';
import { GlobalPermissionsResponse } from '../../types/permissions';

describe('PermissionsService', () => {
  let service: PermissionsService;
  let isAuthenticatedSubject: BehaviorSubject<boolean>;

  let httpClientSpy: { get: ReturnType<typeof vi.fn> };
  let projectServiceSpy: { getPermissions: ReturnType<typeof vi.fn> };
  let authServiceSpy: { isAuthenticated$: BehaviorSubject<boolean> };

  beforeEach(() => {
    isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

    httpClientSpy = {
      get: vi.fn().mockReturnValue(of({ permissions: [] } as GlobalPermissionsResponse)),
    };

    projectServiceSpy = {
      getPermissions: vi.fn().mockReturnValue(of([])),
    };

    authServiceSpy = {
      isAuthenticated$: isAuthenticatedSubject,
    };

    TestBed.configureTestingModule({
      providers: [
        PermissionsService,
        { provide: HttpClient, useValue: httpClientSpy },
        { provide: ProjectService, useValue: projectServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    });

    service = TestBed.inject(PermissionsService);
  });

  it('should return false when user is not authenticated and no project is selected', () => {
    // Assert
    expect(service.hasPermission('project:create')).toBe(false);
    expect(service.hasPermission('gateway_plugin:management')).toBe(false);
  });

  it('should load global permissions when user is authenticated', async () => {
    // Arrange
    const globalPerms: string[] = ['project:create', 'gateway_plugin:management'];
    httpClientSpy.get.mockReturnValue(of({ permissions: globalPerms } as GlobalPermissionsResponse));

    // Act
    isAuthenticatedSubject.next(true);
    await TestBed.inject(PermissionsService);
    TestBed.flushEffects();

    // Assert
    expect(service.hasPermission('project:create')).toBe(true);
    expect(service.hasPermission('gateway_plugin:management')).toBe(true);
    expect(service.hasPermission('model:load')).toBe(false);
  });

  it('should combine global and project permissions', async () => {
    // Arrange
    const globalPerms: string[] = ['project:create'];
    httpClientSpy.get.mockReturnValue(of({ permissions: globalPerms } as GlobalPermissionsResponse));
    const projectPerms: string[] = ['model:load', 'analysis:run'];
    projectServiceSpy.getPermissions.mockReturnValue(of(projectPerms));

    // Act
    isAuthenticatedSubject.next(true);
    service.setProjectId('project-123');
    TestBed.flushEffects();

    // Assert
    expect(service.hasPermission('project:create')).toBe(true);
    expect(service.hasPermission('model:load')).toBe(true);
    expect(service.hasPermission('analysis:run')).toBe(true);
    expect(service.hasPermission('project:delete')).toBe(false);
  });
});
