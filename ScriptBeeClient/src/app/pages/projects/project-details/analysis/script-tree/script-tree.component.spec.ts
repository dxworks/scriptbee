import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ScriptTreeComponent } from './script-tree.component';
import { ProjectStructureService } from '../../../../../services/projects/project-structure.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, Subject, throwError } from 'rxjs';
import { GetProjectFilesResponse, ProjectFileNode } from '../../../../../types/project';
import { HttpErrorResponse } from '@angular/common/http';
import { By } from '@angular/platform-browser';
import { ProjectContextService } from '../../../../../services/projects/project-context.service';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';

const makeFileNode = (overrides: Partial<ProjectFileNode> = {}): ProjectFileNode => ({
  id: 'file-1',
  name: 'script.cs',
  path: '/scripts/script.cs',
  absolutePath: '/abs/scripts/script.cs',
  type: 'file',
  hasChildren: false,
  ...overrides,
});

const makeFolderNode = (overrides: Partial<ProjectFileNode> = {}): ProjectFileNode => ({
  id: 'folder-1',
  name: 'scripts',
  path: '/scripts',
  absolutePath: '/abs/scripts',
  type: 'folder',
  hasChildren: true,
  ...overrides,
});

const makeResponse = (data: ProjectFileNode[], totalCount?: number): GetProjectFilesResponse => ({
  data,
  totalCount: totalCount ?? data.length,
  offset: 0,
  limit: 50,
});

function createProjectStructureServiceMock() {
  return {
    getProjectFiles: vi.fn().mockReturnValue(of(makeResponse([]))),
    deleteProjectStructureNode: vi.fn().mockReturnValue(of(null)),
    updateProjectScript: vi.fn().mockReturnValue(of({ id: 'file-1', name: 'renamed.cs' })),
  };
}

function createDialogMock() {
  const dialogRef = { afterClosed: vi.fn().mockReturnValue(of(undefined)) };
  return {
    open: vi.fn().mockReturnValue(dialogRef),
    dialogRef,
  };
}

function getRenameButton() {
  const menuButtons = document.querySelectorAll('.mat-mdc-menu-item');
  return Array.from(menuButtons).find((b) => b.textContent?.includes('Rename')) as HTMLButtonElement | undefined;
}

function getDeleteButton() {
  const menuButtons = document.querySelectorAll('.mat-mdc-menu-item');
  return Array.from(menuButtons).find((b) => b.textContent?.includes('Delete')) as HTMLButtonElement | undefined;
}

function clickActionsMenu(fixture: ComponentFixture<ScriptTreeComponent>) {
  const menuTrigger = fixture.debugElement.query(By.css('app-tree-actions-menu button'));
  menuTrigger.nativeElement.click();
  fixture.detectChanges();
}

describe('ScriptTreeComponent', () => {
  let component: ScriptTreeComponent;
  let fixture: ComponentFixture<ScriptTreeComponent>;
  let projectStructureService: ReturnType<typeof createProjectStructureServiceMock>;
  let projectContextService: { generateClasses: ReturnType<typeof vi.fn> };
  let projectStateService: { currentInstanceId: ReturnType<typeof vi.fn> };
  let dialog: ReturnType<typeof createDialogMock>;
  let snackbar: { open: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    projectStructureService = createProjectStructureServiceMock();
    projectContextService = { generateClasses: vi.fn().mockReturnValue(of(undefined)) };
    projectStateService = { currentInstanceId: vi.fn().mockReturnValue('instance-123') };
    dialog = createDialogMock();
    snackbar = { open: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [ScriptTreeComponent],
      providers: [
        { provide: ProjectStructureService, useValue: projectStructureService },
        { provide: ProjectContextService, useValue: projectContextService },
        { provide: ProjectStateService, useValue: projectStateService },
        { provide: MatDialog, useValue: dialog },
        { provide: MatSnackBar, useValue: snackbar },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ScriptTreeComponent);
    component = fixture.componentInstance;
  });

  describe('Initialization', () => {
    it('should render the create new script button', () => {
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();

      const createButton = fixture.debugElement.query(By.css('button[aria-label="create new script"]'));
      expect(createButton).toBeTruthy();
    });

    it('should render the reload tree button', () => {
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();

      const reloadButton = fixture.debugElement.query(By.css('button[aria-label="reload tree"]'));
      expect(reloadButton).toBeTruthy();
    });

    it('should pass fetchData successfully to lazy file tree', () => {
      const file = makeFileNode();
      const folder = makeFolderNode();
      projectStructureService.getProjectFiles.mockReturnValue(of(makeResponse([folder, file])));
      fixture.componentRef.setInput('projectId', 'project-abc');

      fixture.detectChanges();

      const treeText = fixture.nativeElement.textContent;
      expect(treeText).toContain('script.cs');
      expect(treeText).toContain('scripts');
      expect(projectStructureService.getProjectFiles).toHaveBeenCalledWith('project-abc', undefined, 0, 50);
    });
  });

  describe('Interaction', () => {
    it('should open the create script dialog when the create button is clicked', () => {
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();
      const createButton = fixture.debugElement.query(By.css('button[aria-label="create new script"]'));

      createButton.nativeElement.click();

      expect(dialog.open).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ data: { projectId: 'project-abc' } }));
    });

    it('should fetch folder children and render them when a folder is expanded', () => {
      const folder = makeFolderNode({ id: 'folder-a' });
      projectStructureService.getProjectFiles.mockReturnValueOnce(of(makeResponse([folder])));
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();
      const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
      projectStructureService.getProjectFiles.mockReturnValueOnce(of(makeResponse([makeFileNode({ name: 'child.cs' })])));

      toggleButton.nativeElement.click();
      fixture.detectChanges();

      expect(projectStructureService.getProjectFiles).toHaveBeenCalledWith('project-abc', 'folder-a', 0, 50);
      expect(fixture.nativeElement.textContent).toContain('child.cs');
    });

    it('should emit fileSelected event when a file node is clicked', () => {
      const file = makeFileNode();
      projectStructureService.getProjectFiles.mockReturnValue(of(makeResponse([file])));
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();
      const fileSelectedSpy = vi.spyOn(component.fileSelected, 'emit');
      const fileNode = fixture.debugElement.query(By.css('.leaf-node'));

      fileNode.nativeElement.parentElement.click();

      expect(fileSelectedSpy).toHaveBeenCalledWith(file);
    });

    it('should generate classes when generate classes button is clicked', () => {
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();
      const generateButton = fixture.debugElement.query(By.css('button[aria-label="generate classes"]'));

      generateButton.nativeElement.click();

      expect(projectContextService.generateClasses).toHaveBeenCalledWith('project-abc', 'instance-123');
      expect(snackbar.open).toHaveBeenCalledWith('Successfully generated model classes', 'Dismiss', expect.anything());
    });

    it('should show error when generate classes fails', () => {
      fixture.componentRef.setInput('projectId', 'project-abc');
      const httpError = new HttpErrorResponse({ error: { title: 'Unknown Error', status: 500 }, status: 500 });
      projectContextService.generateClasses.mockReturnValue(throwError(() => httpError));
      fixture.detectChanges();
      const generateButton = fixture.debugElement.query(By.css('button[aria-label="generate classes"]'));

      generateButton.nativeElement.click();

      expect(projectContextService.generateClasses).toHaveBeenCalledWith('project-abc', 'instance-123');
      expect(snackbar.open).toHaveBeenCalledWith(expect.stringContaining('Unknown Error'), 'Dismiss', expect.anything());
    });
  });

  describe('Actions', () => {
    it('should call deleteProjectStructureNode and remove node from tree', () => {
      const file = makeFileNode();
      projectStructureService.getProjectFiles.mockReturnValue(of(makeResponse([file])));
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();
      clickActionsMenu(fixture);
      const deleteButton = getDeleteButton();
      projectStructureService.getProjectFiles.mockClear();

      deleteButton!.click();
      fixture.detectChanges();

      expect(projectStructureService.deleteProjectStructureNode).toHaveBeenCalledWith('project-abc', 'file-1');
      expect(projectStructureService.getProjectFiles).not.toHaveBeenCalled();
      expect(fixture.nativeElement.textContent).not.toContain('script.cs');
    });

    it('should display the loading progress bar when a deletion is in progress', async () => {
      const file = makeFileNode();
      projectStructureService.getProjectFiles.mockReturnValue(of(makeResponse([file])));
      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();
      clickActionsMenu(fixture);
      const deleteButton = getDeleteButton();
      const deletionObservable = new Subject<void>();
      projectStructureService.deleteProjectStructureNode.mockReturnValue(deletionObservable.asObservable());

      deleteButton!.click();
      fixture.detectChanges();

      let progressBar = fixture.debugElement.query(By.css('app-loading-progress-bar'));
      expect(progressBar).toBeTruthy();

      deletionObservable.next();
      deletionObservable.complete();

      await fixture.whenStable();
      fixture.detectChanges();

      progressBar = fixture.debugElement.query(By.css('app-loading-progress-bar'));
      expect(progressBar).toBeFalsy();
    });

    it('should update the node name in the lazy tree when rename succeeds', async () => {
      const file = makeFileNode({ id: 'file-1', name: 'original.cs' });
      projectStructureService.getProjectFiles.mockReturnValue(of(makeResponse([file])));
      projectStructureService.updateProjectScript.mockReturnValue(of({ ...file, name: 'renamed.cs' }));

      dialog.dialogRef.afterClosed.mockReturnValue(of('renamed.cs'));

      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();

      clickActionsMenu(fixture);
      const renameButton = getRenameButton();
      const initialCallCount = projectStructureService.getProjectFiles.mock.calls.length;

      renameButton!.click();
      fixture.detectChanges();

      expect(projectStructureService.getProjectFiles.mock.calls.length).toBe(initialCallCount);
      expect(fixture.nativeElement.textContent).toContain('renamed.cs');
      expect(fixture.nativeElement.textContent).not.toContain('original.cs');
    });

    it('should show a snackbar when rename fails', () => {
      const file = makeFileNode();
      projectStructureService.getProjectFiles.mockReturnValue(of(makeResponse([file])));
      const httpError = new HttpErrorResponse({ error: { title: 'Conflict Error', status: 409 }, status: 409 });
      projectStructureService.updateProjectScript.mockReturnValue(throwError(() => httpError));

      dialog.dialogRef.afterClosed.mockReturnValue(of('renamed.cs'));

      fixture.componentRef.setInput('projectId', 'project-abc');
      fixture.detectChanges();

      clickActionsMenu(fixture);
      const renameButton = getRenameButton();

      renameButton!.click();
      fixture.detectChanges();

      expect(snackbar.open).toHaveBeenCalledWith(expect.stringContaining('Conflict Error'), 'Dismiss', expect.anything());
    });
  });
});
