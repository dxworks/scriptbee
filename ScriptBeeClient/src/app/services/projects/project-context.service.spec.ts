import { beforeEach, describe, expect, it } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ProjectContextService } from './project-context.service';
import { ContextGraphResult } from '../../types/context-graph';

describe('ProjectContextService', () => {
  let service: ProjectContextService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProjectContextService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ProjectContextService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch graph nodes from server when searching', () => {
    const mockResult: ContextGraphResult = { nodes: [], edges: [] };
    const projectId = 'p1';
    const instanceId = 'i1';
    const query = 'test';

    service.searchNodes(projectId, instanceId, query).subscribe((result) => {
      expect(result).toEqual(mockResult);
    });

    const req = httpMock.expectOne((r) => r.url.includes('/context/graph') && r.url.includes(`query=${query}`));
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);

    httpMock.verify();
  });

  it('should request neighbors for a specific node', () => {
    const mockResult: ContextGraphResult = { nodes: [], edges: [] };
    const projectId = 'p1';
    const instanceId = 'i1';
    const nodeId = 'node-123';

    service.getNeighbors(projectId, instanceId, nodeId).subscribe((result) => {
      expect(result).toEqual(mockResult);
    });

    const req = httpMock.expectOne((r) => r.url.includes(`/context/graph/neighbors`) && r.url.includes(`nodeId=${encodeURIComponent(nodeId)}`));
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);

    httpMock.verify();
  });
});
