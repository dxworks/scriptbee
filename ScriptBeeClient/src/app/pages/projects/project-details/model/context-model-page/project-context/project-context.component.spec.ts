import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectContextComponent } from './project-context.component';
import { ProjectContextService } from '../../../../../../services/projects/project-context.service';
import { of } from 'rxjs';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { By } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('ProjectContextComponent', () => {
  let fixture: ComponentFixture<ProjectContextComponent>;
  let projectContextService: {
    getProjectContext: ReturnType<typeof vi.fn>;
    reloadContext: ReturnType<typeof vi.fn>;
    clearContext: ReturnType<typeof vi.fn>;
    generateClasses: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    projectContextService = {
      getProjectContext: vi.fn().mockReturnValue(of([])),
      reloadContext: vi.fn().mockReturnValue(of(undefined)),
      clearContext: vi.fn().mockReturnValue(of(undefined)),
      generateClasses: vi.fn().mockReturnValue(of(undefined)),
    };

    await TestBed.configureTestingModule({
      imports: [ProjectContextComponent],
      providers: [{ provide: ProjectContextService, useValue: projectContextService }, provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectContextComponent);
    fixture.componentRef.setInput('projectId', 'proj-1');
    fixture.componentRef.setInput('instanceId', 'inst-1');
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it('should call reloadContext when reload button is clicked', () => {
    const reloadButton = fixture.debugElement.query(By.css('.context-actions-row button:first-of-type'));
    reloadButton.nativeElement.click();

    expect(projectContextService.reloadContext).toHaveBeenCalledWith('proj-1', 'inst-1');
  });

  it('should call clearContext when clear button is clicked', () => {
    const clearButton = fixture.debugElement.query(By.css('.context-actions-row button:last-of-type'));
    clearButton.nativeElement.click();

    expect(projectContextService.clearContext).toHaveBeenCalledWith('proj-1', 'inst-1');
  });
});
