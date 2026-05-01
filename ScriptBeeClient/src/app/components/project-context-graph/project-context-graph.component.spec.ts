import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectContextGraphComponent } from './project-context-graph.component';
import { ProjectContextService } from '../../services/projects/project-context.service';
import { of } from 'rxjs';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

describe('ProjectContextGraphComponent', () => {
  let component: ProjectContextGraphComponent;
  let fixture: ComponentFixture<ProjectContextGraphComponent>;
  let projectContextServiceMock: {
    searchNodes: ReturnType<typeof vi.fn>;
    getNeighbors: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    projectContextServiceMock = {
      searchNodes: vi.fn().mockReturnValue(of({ nodes: [], edges: [] })),
      getNeighbors: vi.fn().mockReturnValue(of({ nodes: [], edges: [] })),
    };

    await TestBed.configureTestingModule({
      imports: [ProjectContextGraphComponent],
      providers: [{ provide: ProjectContextService, useValue: projectContextServiceMock }, provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectContextGraphComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('projectId', 'p1');
    fixture.componentRef.setInput('instanceId', 'i1');
    fixture.detectChanges();
  });

  it('should create the graph component', () => {
    expect(component).toBeTruthy();
  });

  it('should provide a search bar for users to find models', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const searchInput = compiled.querySelector('input[placeholder="e.g. MyClass"]');
    expect(searchInput).toBeTruthy();
  });

  it('should show an empty state message when no nodes are loaded', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Search for nodes above to start exploring the project context.');
  });
});
