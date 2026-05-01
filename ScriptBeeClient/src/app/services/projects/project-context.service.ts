import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, retry } from 'rxjs';
import { ProjectContext } from '../../types/returned-context-slice';
import { WebResponse } from '../../types/web-response';
import { ContextGraphResult } from '../../types/context-graph';

@Injectable({
  providedIn: 'root',
})
export class ProjectContextService {
  private http = inject(HttpClient);

  getProjectContext(projectId: string, instanceId: string): Observable<ProjectContext> {
    return this.http.get<WebResponse<ProjectContext>>(`/api/projects/${projectId}/instances/${instanceId}/context`).pipe(
      retry({ count: 3, delay: 1000 }),
      map((res) => res.data)
    );
  }

  clearContext(projectId: string, instanceId: string) {
    return this.http.post<void>(`/api/projects/${projectId}/instances/${instanceId}/context/clear`, {});
  }

  reloadContext(projectId: string, instanceId: string) {
    return this.http.post<void>(`/api/projects/${projectId}/instances/${instanceId}/context/reload`, {});
  }

  searchNodes(projectId: string, instanceId: string, query: string, offset = 0, limit = 100): Observable<ContextGraphResult> {
    return this.http.get<ContextGraphResult>(`/api/projects/${projectId}/instances/${instanceId}/context/graph?query=${query}&offset=${offset}&limit=${limit}`);
  }

  getNeighbors(projectId: string, instanceId: string, nodeId: string): Observable<ContextGraphResult> {
    return this.http.get<ContextGraphResult>(`/api/projects/${projectId}/instances/${instanceId}/context/graph/neighbors?nodeId=${encodeURIComponent(nodeId)}`);
  }
}
