import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ManageAccessComponent } from './manage-access.component';
import { ProjectService } from '../../../../services/projects/project.service';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';
import { Project } from '../../../../types/project';
import { signal } from '@angular/core';

describe('ManageAccessComponent', () => {
  let fixture: ComponentFixture<ManageAccessComponent>;

  const projectServiceSpy = {
    getProjectMembers: vi.fn(),
    getProjectTokens: vi.fn(),
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

  beforeEach(async () => {
    projectServiceSpy.getProjectMembers.mockReset().mockReturnValue(of([]));
    projectServiceSpy.getProjectTokens.mockReset().mockReturnValue(of([]));
    projectServiceSpy.getAllUsers.mockReset().mockReturnValue(of([]));
    projectServiceSpy.getRoles.mockReset().mockReturnValue(of([]));

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
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should render both project members and tokens sections', () => {
    const membersElement = fixture.debugElement.query(By.css('app-project-members'));
    const tokensElement = fixture.debugElement.query(By.css('app-project-tokens'));

    expect(membersElement).toBeTruthy();
    expect(tokensElement).toBeTruthy();
  });
});
