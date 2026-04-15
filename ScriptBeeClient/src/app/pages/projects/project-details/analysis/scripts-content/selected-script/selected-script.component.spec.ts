import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SelectedScriptComponent } from './selected-script.component';
import { ProjectStructureService } from '../../../../../../services/projects/project-structure.service';
import { ThemeService } from '../../../../../../services/common/theme.service';
import { of, throwError, Subject } from 'rxjs';
import { signal } from '@angular/core';
import { describe, expect, it, beforeEach, vi, Mock, afterEach } from 'vitest';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AnalysisService } from '../../../../../../services/analysis/analysis.service';
import { provideHttpClient } from '@angular/common/http';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { ProjectLiveUpdatesService } from '../../../../../../services/projects/project-live-updates.service';
import { ScriptUpdatedEvent } from '../../../../../../types/live-updates';

interface MockServices {
  projectStructure: {
    getProjectScript: Mock;
    getScriptContent: Mock;
    updateScriptContent: Mock;
  };
  liveUpdates: {
    scriptUpdated$: Subject<ScriptUpdatedEvent>;
  };
}

describe('SelectedScriptComponent', () => {
  let component: SelectedScriptComponent;
  let fixture: ComponentFixture<SelectedScriptComponent>;
  let mocks: MockServices;

  beforeEach(async () => {
    mocks = {
      projectStructure: {
        getProjectScript: vi.fn().mockReturnValue(of({ scriptLanguage: { name: 'csharp' } })),
        getScriptContent: vi.fn().mockReturnValue(of('initial content')),
        updateScriptContent: vi.fn().mockReturnValue(of({})),
      },
      liveUpdates: {
        scriptUpdated$: new Subject<ScriptUpdatedEvent>(),
      },
    };

    await TestBed.configureTestingModule({
      imports: [SelectedScriptComponent, FormsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideMonacoEditor(),
        { provide: ProjectStructureService, useValue: mocks.projectStructure },
        { provide: ThemeService, useValue: { darkMode: signal(false) } },
        { provide: MatDialog, useValue: { open: vi.fn() } },
        { provide: MatSnackBar, useValue: { open: vi.fn() } },
        { provide: AnalysisService, useValue: { triggerAnalysis: vi.fn() } },
        { provide: ProjectLiveUpdatesService, useValue: mocks.liveUpdates },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SelectedScriptComponent);
    component = fixture.componentInstance;

    fixture.componentRef.setInput('projectId', 'proj-1');
    fixture.componentRef.setInput('instanceId', 'inst-1');
    fixture.componentRef.setInput('scriptId', 'script-1');

    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
  });

  afterEach(() => {
    vi.clearAllMocks();
    vi.useRealTimers();
  });

  it('should enable editing', () => {
    expect(component.editorOptions().readOnly).toBe(false);
  });

  it('should show Saving and then Saved after debounce', async () => {
    const updateSubject = new Subject<object>();
    mocks.projectStructure.updateScriptContent.mockReturnValue(updateSubject);
    vi.useFakeTimers();

    const editor = fixture.debugElement.query(By.css('ngx-monaco-editor'));
    editor.triggerEventHandler('ngModelChange', 'new content');

    await vi.advanceTimersByTimeAsync(1001);
    expect(component.saveStatus()).toBe('Saving...');

    updateSubject.next({});
    expect(component.saveStatus()).toBe('Saved');
    expect(mocks.projectStructure.updateScriptContent).toHaveBeenCalledWith('proj-1', 'script-1', 'new content');
  });

  it('should handle errors and allow subsequent updates', async () => {
    vi.useFakeTimers();
    mocks.projectStructure.updateScriptContent.mockReturnValue(throwError(() => new Error('failed')));

    const editor = fixture.debugElement.query(By.css('ngx-monaco-editor'));
    editor.triggerEventHandler('ngModelChange', 'bad content');

    await vi.advanceTimersByTimeAsync(1001);
    expect(component.saveStatus()).toBe('Error');

    mocks.projectStructure.updateScriptContent.mockReturnValue(of({}));
    editor.triggerEventHandler('ngModelChange', 'good content');

    await vi.advanceTimersByTimeAsync(1001);
    expect(component.saveStatus()).toBe('Saved');
    expect(mocks.projectStructure.updateScriptContent).toHaveBeenCalledWith('proj-1', 'script-1', 'good content');
  });

  it('should reload content when script is updated remotely', async () => {
    mocks.projectStructure.getScriptContent.mockReturnValue(of('remote content'));

    mocks.liveUpdates.scriptUpdated$.next({ scriptId: 'script-1', projectId: 'proj-1', clientId: 'other' });

    expect(mocks.projectStructure.getScriptContent).toHaveBeenCalledWith('proj-1', 'script-1');
  });
});
