import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectTokensComponent } from './project-tokens.component';
import { ProjectService } from '../../../../../services/projects/project.service';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';
import { CreateTokenResponse, ProjectToken, RoleInfo } from '../../../../../types/project';
import { MatSnackBar } from '@angular/material/snack-bar';

describe('ProjectTokensComponent', () => {
  let fixture: ComponentFixture<ProjectTokensComponent>;
  let snackbarSpy: { open: ReturnType<typeof vi.fn> };

  const projectServiceSpy = {
    getProjectTokens: vi.fn(),
    createProjectToken: vi.fn(),
    deleteProjectToken: vi.fn(),
    getRoles: vi.fn(),
  };

  const mockTokens: ProjectToken[] = [
    {
      id: 'token-1',
      description: 'CI Runner Token',
      role: 'editor',
      createdAt: '2026-01-01T00:00:00Z',
      expiresAt: '2026-12-31T00:00:00Z',
    },
  ];

  const mockRoles: RoleInfo[] = [
    { id: 'owner', description: 'Full control over the project' },
    { id: 'editor', description: 'Can modify project resources' },
  ];

  beforeEach(async () => {
    projectServiceSpy.getProjectTokens.mockReset().mockReturnValue(of(mockTokens));
    projectServiceSpy.createProjectToken.mockReset();
    projectServiceSpy.deleteProjectToken.mockReset().mockReturnValue(of(undefined));
    projectServiceSpy.getRoles.mockReset().mockReturnValue(of(mockRoles));

    snackbarSpy = {
      open: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [ProjectTokensComponent],
      providers: [{ provide: ProjectService, useValue: projectServiceSpy }],
    })
      .overrideComponent(ProjectTokensComponent, {
        add: {
          providers: [{ provide: MatSnackBar, useValue: snackbarSpy }],
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(ProjectTokensComponent);
    fixture.componentRef.setInput('projectId', 'project-1');
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should display existing tokens in the table', () => {
    // Arrange & Act
    const content = fixture.nativeElement.textContent;

    // Assert
    expect(content).toContain('CI Runner Token');
    expect(content).toContain('editor');
  });

  it('should create token when form is valid and submitted', async () => {
    // Arrange
    const component = fixture.componentInstance;
    const mockCreatedResponse: CreateTokenResponse = {
      id: 'token-2',
      token: 'sb_secret_token_123',
      description: 'New Pipeline',
      role: 'editor',
      createdAt: '2026-01-01T00:00:00Z',
      expiresAt: '2026-12-31T00:00:00Z',
    };
    projectServiceSpy.createProjectToken.mockReturnValue(of(mockCreatedResponse));

    component.createTokenForm.controls.description.setValue('New Pipeline');
    component.createTokenForm.controls.role.setValue(mockRoles[1]);
    component.createTokenForm.controls.expiresAt.setValue(new Date('2026-12-31T00:00:00Z'));
    fixture.detectChanges();
    await fixture.whenStable();

    // Act
    const submitBtn = fixture.debugElement.query(By.css('#create-token-submit'));
    submitBtn.nativeElement.click();
    fixture.detectChanges();
    await fixture.whenStable();

    // Assert
    expect(projectServiceSpy.createProjectToken).toHaveBeenCalledWith('project-1', {
      description: 'New Pipeline',
      role: 'editor',
      expiresAt: new Date('2026-12-31T00:00:00Z').toISOString(),
    });
    expect(snackbarSpy.open).toHaveBeenCalledWith('Token created.', 'Dismiss', { duration: 3000 });
    expect(fixture.nativeElement.textContent).toContain('sb_secret_token_123');
  });

  it('should delete token when delete button is clicked', async () => {
    // Arrange
    const deleteBtn = fixture.debugElement.query(By.css('#delete-token-token-1'));

    // Act
    deleteBtn.triggerEventHandler('click', null);
    fixture.detectChanges();
    await fixture.whenStable();

    // Assert
    expect(projectServiceSpy.deleteProjectToken).toHaveBeenCalledWith('project-1', 'token-1');
    expect(snackbarSpy.open).toHaveBeenCalledWith('Token deleted.', 'Dismiss', { duration: 3000 });
  });
});
