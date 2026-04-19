import { beforeEach, describe, expect, it, Mock, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ScriptTreeComponent } from './script-tree.component';
import { ProjectStructureService } from '../../../../../services/projects/project-structure.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, Subject } from 'rxjs';
import { GetProjectFilesResponse, ProjectFileNode } from '../../../../../types/project';
import { By } from '@angular/platform-browser';
import { ProjectContextService } from '../../../../../services/projects/project-context.service';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';
import { ProjectLiveUpdatesService } from '../../../../../services/projects/project-live-updates.service';
import { ScriptCreateEvent, ScriptDeletedEvent, ScriptUpdatedEvent } from '../../../../../types/live-updates';

const makeFileNode = (overrides: Partial<ProjectFileNode> = {}): ProjectFileNode => ({
  id: 'file-1',
  name: 'script.cs',
  path: '/scripts/script.cs',
  type: 'file',
  hasChildren: false,
  ...overrides,
});

const makeResponse = (data: ProjectFileNode[]): GetProjectFilesResponse => ({
  data,
  totalCount: data.length,
  offset: 0,
  limit: 50,
});

interface MockServices {
  projectStructure: {
    getProjectFiles: Mock;
    deleteProjectStructureNode: Mock;
    updateProjectScript: Mock;
    getProjectScript: Mock;
  };
  dialog: {
    open: Mock;
  };
  snackbar: {
    open: Mock;
  };
  liveUpdates: {
    scriptCreated$: Subject<ScriptCreateEvent>;
    scriptDeleted$: Subject<ScriptDeletedEvent>;
    scriptUpdated$: Subject<ScriptUpdatedEvent>;
  };
}

describe('ScriptTreeComponent', () => {
  let component: ScriptTreeComponent;
  let fixture: ComponentFixture<ScriptTreeComponent>;
  let mocks: MockServices;

  beforeEach(async () => {
    mocks = {
      projectStructure: {
        getProjectFiles: vi.fn().mockReturnValue(of(makeResponse([]))),
        deleteProjectStructureNode: vi.fn().mockReturnValue(of(null)),
        updateProjectScript: vi.fn(),
        getProjectScript: vi.fn(),
      },
      dialog: {
        open: vi.fn().mockReturnValue({ afterClosed: () => of(undefined) }),
      },
      snackbar: {
        open: vi.fn(),
      },
      liveUpdates: {
        scriptCreated$: new Subject<ScriptCreateEvent>(),
        scriptDeleted$: new Subject<ScriptDeletedEvent>(),
        scriptUpdated$: new Subject<ScriptUpdatedEvent>(),
      },
    };

    await TestBed.configureTestingModule({
      imports: [ScriptTreeComponent],
      providers: [
        { provide: ProjectStructureService, useValue: mocks.projectStructure },
        { provide: ProjectContextService, useValue: { generateClasses: vi.fn().mockReturnValue(of(undefined)) } },
        { provide: ProjectStateService, useValue: { currentInstanceId: vi.fn().mockReturnValue('inst-1') } },
        { provide: MatDialog, useValue: mocks.dialog },
        { provide: MatSnackBar, useValue: mocks.snackbar },
        { provide: ProjectLiveUpdatesService, useValue: mocks.liveUpdates },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ScriptTreeComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('projectId', 'proj-1');
    fixture.componentRef.setInput('selectedFileId', null);
    fixture.detectChanges();
  });

  it('should render the control buttons', () => {
    const createBtn = fixture.debugElement.query(By.css('button[aria-label="create new script"]'));
    const reloadBtn = fixture.debugElement.query(By.css('button[aria-label="reload tree"]'));

    expect(createBtn).toBeTruthy();
    expect(reloadBtn).toBeTruthy();
  });

  describe('Live Updates', () => {
    it('should reload root on ScriptCreated event', () => {
      const reloadFolderSpy = vi.spyOn(component.lazyTree(), 'reloadFolder');

      mocks.liveUpdates.scriptCreated$.next({ scriptId: 'new-id', projectId: 'proj-1', clientId: 'other' });

      expect(reloadFolderSpy).toHaveBeenCalledWith(null);
    });

    it('should remove node on ScriptDeleted event', () => {
      const removeNodeSpy = vi.spyOn(component.lazyTree(), 'removeNode');

      mocks.liveUpdates.scriptDeleted$.next({ scriptId: 'del-id', projectId: 'proj-1', clientId: 'other' });

      expect(removeNodeSpy).toHaveBeenCalledWith('del-id');
    });

    it('should update node on ScriptUpdated event', () => {
      const updateNodeSpy = vi.spyOn(component.lazyTree(), 'updateNode');
      const scriptData = makeFileNode({ id: 'upd-id', name: 'updated.cs' });
      mocks.projectStructure.getProjectScript.mockReturnValue(of(scriptData));

      mocks.liveUpdates.scriptUpdated$.next({ scriptId: 'upd-id', projectId: 'proj-1', clientId: 'other' });

      expect(mocks.projectStructure.getProjectScript).toHaveBeenCalledWith('proj-1', 'upd-id');
      expect(updateNodeSpy).toHaveBeenCalledWith('upd-id', expect.any(Function));
    });
  });
});
