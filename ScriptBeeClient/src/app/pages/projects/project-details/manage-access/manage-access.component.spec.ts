import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ManageAccessComponent } from './manage-access.component';
import { ProjectService } from '../../../../services/projects/project.service';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';
import { Project, ProjectMember, RoleInfo, UserInfo } from '../../../../types/project';
import { signal } from '@angular/core';
import { MatSnackBarRef, TextOnlySnackBar } from '@angular/material/snack-bar';

describe('ManageAccessComponent', () => {
  let component: ManageAccessComponent;
  let fixture: ComponentFixture<ManageAccessComponent>;
  let snackbarOpenSpy: unknown;

  const projectServiceSpy = {
    getProjectMembers: vi.fn(),
    updateProjectMember: vi.fn(),
    removeProjectMember: vi.fn(),
    getAllUsers: vi.fn(),
    getRoles: vi.fn(),
  };

  const mockProject: Project = {
    id: 'project-1',
    name: 'Test Project',
    creationDate: '2024-02-08',
    savedFiles: {},
    loadedFiles: {},
    linkers: [],
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

    const projectStateServiceMock = {
      currentProject: signal<Project | null>(mockProject),
    };

    await TestBed.configureTestingModule({
      imports: [ManageAccessComponent],
      providers: [
        { provide: ProjectService, useValue: projectServiceSpy },
        { provide: ProjectStateService, useValue: projectStateServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ManageAccessComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
    snackbarOpenSpy = vi.spyOn(component.snackbar, 'open').mockReturnValue({} as MatSnackBarRef<TextOnlySnackBar>);

    await fixture.whenStable();
  });

  describe('Business Perspective: Manage Project Access Page', () => {
    it('should show current members and groups with their roles in the table', () => {
      const screenText = fixture.nativeElement.textContent;
      expect(screenText).toContain('user-a');
      expect(screenText).toContain('team-b');
      expect(screenText).toContain('owner');
      expect(screenText).toContain('viewer');
    });

    it('should add/update group member access when user selects group details and clicks submit', async () => {
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

      const submitBtn = fixture.debugElement.query(By.css('#add-member-submit'));
      submitBtn.nativeElement.click();

      fixture.detectChanges();
      await fixture.whenStable();

      expect(projectServiceSpy.updateProjectMember).toHaveBeenCalledWith('project-1', 'dev-team', 'editor', 'group');
      expect(snackbarOpenSpy).toHaveBeenCalledWith('Member access updated.', 'Dismiss', { duration: 3000 });
    });

    it('should revoke access when the user clicks the delete button for a member', async () => {
      const deleteBtn = fixture.debugElement.query(By.css('#remove-member-user-a'));
      deleteBtn.triggerEventHandler('click', null);

      fixture.detectChanges();
      await fixture.whenStable();

      expect(projectServiceSpy.removeProjectMember).toHaveBeenCalledWith('project-1', 'user-a', 'user');
      expect(snackbarOpenSpy).toHaveBeenCalledWith('Member removed.', 'Dismiss', { duration: 3000 });
    });
  });
});
