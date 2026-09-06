import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectMembersComponent } from './project-members.component';
import { ProjectService } from '../../../../../services/projects/project.service';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';
import { ProjectMember, RoleInfo, UserInfo } from '../../../../../types/project';
import { MatSnackBar } from '@angular/material/snack-bar';

describe('ProjectMembersComponent', () => {
  let fixture: ComponentFixture<ProjectMembersComponent>;
  let snackbarSpy: { open: ReturnType<typeof vi.fn> };

  const projectServiceSpy = {
    getProjectMembers: vi.fn(),
    updateProjectMember: vi.fn(),
    removeProjectMember: vi.fn(),
    getAllUsers: vi.fn(),
    getRoles: vi.fn(),
  };

  const mockMembers: ProjectMember[] = [
    { memberId: 'user-a', memberType: 'user', role: 'owner' },
    { memberId: 'team-b', memberType: 'group', role: 'viewer' },
  ];

  const mockUsers: UserInfo[] = [
    { id: 'user-a', name: 'Alice' },
    { id: 'user-c', name: 'Charlie' },
  ];

  const mockRoles: RoleInfo[] = [
    { id: 'owner', description: 'Full control over the project' },
    { id: 'editor', description: 'Can modify project resources' },
    { id: 'viewer', description: 'Read-only access to the project' },
  ];

  beforeEach(async () => {
    projectServiceSpy.getProjectMembers.mockReset().mockReturnValue(of(mockMembers));
    projectServiceSpy.updateProjectMember.mockReset().mockReturnValue(of(undefined));
    projectServiceSpy.removeProjectMember.mockReset().mockReturnValue(of(undefined));
    projectServiceSpy.getAllUsers.mockReset().mockReturnValue(of(mockUsers));
    projectServiceSpy.getRoles.mockReset().mockReturnValue(of(mockRoles));

    snackbarSpy = {
      open: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [ProjectMembersComponent],
      providers: [{ provide: ProjectService, useValue: projectServiceSpy }],
    })
      .overrideComponent(ProjectMembersComponent, {
        add: {
          providers: [{ provide: MatSnackBar, useValue: snackbarSpy }],
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(ProjectMembersComponent);
    fixture.componentRef.setInput('projectId', 'project-1');
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should display member details in the table', () => {
    // Arrange & Act
    const content = fixture.nativeElement.textContent;

    // Assert
    expect(content).toContain('user-a');
    expect(content).toContain('team-b');
    expect(content).toContain('owner');
    expect(content).toContain('viewer');
  });

  it('should add or update group member access when user inputs valid details', async () => {
    // Arrange
    const component = fixture.componentInstance;
    component.addMemberForm.controls.memberType.setValue('group');
    fixture.detectChanges();
    await fixture.whenStable();

    const groupInput = fixture.debugElement.query(By.css('#member-id-input')).nativeElement;
    groupInput.value = 'dev-team';
    groupInput.dispatchEvent(new Event('input'));

    const editorRole = mockRoles.find((r) => r.id === 'editor')!;
    component.addMemberForm.controls.role.setValue(editorRole);
    fixture.detectChanges();
    await fixture.whenStable();

    // Act
    const submitBtn = fixture.debugElement.query(By.css('#add-member-submit'));
    submitBtn.nativeElement.click();
    fixture.detectChanges();
    await fixture.whenStable();

    // Assert
    expect(projectServiceSpy.updateProjectMember).toHaveBeenCalledWith('project-1', 'dev-team', 'editor', 'group');
    expect(snackbarSpy.open).toHaveBeenCalledWith('Member access updated.', 'Dismiss', { duration: 3000 });
  });

  it('should remove member when delete button is clicked', async () => {
    // Arrange
    const deleteBtn = fixture.debugElement.query(By.css('#remove-member-user-a'));

    // Act
    deleteBtn.triggerEventHandler('click', null);
    fixture.detectChanges();
    await fixture.whenStable();

    // Assert
    expect(projectServiceSpy.removeProjectMember).toHaveBeenCalledWith('project-1', 'user-a', 'user');
    expect(snackbarSpy.open).toHaveBeenCalledWith('Member removed.', 'Dismiss', { duration: 3000 });
  });
});
