import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { By } from '@angular/platform-browser';
import { HasPermissionDirective } from './has-permission.directive';
import { PermissionsService } from '../services/auth/permissions.service';

@Component({
  standalone: true,
  imports: [HasPermissionDirective],
  template: `
    <button *hasPermission="'gateway_plugin:management'" id="admin-button">Admin Button</button>
    <button *hasPermission="'project:create'" id="create-button">Create Project Button</button>
  `,
})
class TestHostComponent {}

describe('HasPermissionDirective', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let permissionsServiceMock: {
    hasPermission: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    permissionsServiceMock = {
      hasPermission: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
      providers: [{ provide: PermissionsService, useValue: permissionsServiceMock }],
    }).compileComponents();
  });

  it('should render element when permission is granted', () => {
    // Arrange
    permissionsServiceMock.hasPermission.mockImplementation((perm: string) => perm === 'gateway_plugin:management');

    // Act
    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();

    // Assert
    const adminButton = fixture.debugElement.query(By.css('#admin-button'));
    const createButton = fixture.debugElement.query(By.css('#create-button'));

    expect(adminButton).toBeTruthy();
    expect(createButton).toBeFalsy();
  });

  it('should not render element when permission is denied', () => {
    // Arrange
    permissionsServiceMock.hasPermission.mockReturnValue(false);

    // Act
    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();

    // Assert
    const adminButton = fixture.debugElement.query(By.css('#admin-button'));
    const createButton = fixture.debugElement.query(By.css('#create-button'));

    expect(adminButton).toBeFalsy();
    expect(createButton).toBeFalsy();
  });
});
