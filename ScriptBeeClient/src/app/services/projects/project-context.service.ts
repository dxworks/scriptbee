import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ProjectContext } from '../../types/returned-context-slice';
import { WebResponse } from '../../types/web-response';

@Injectable({
  providedIn: 'root',
})
export class ProjectContextService {
  constructor(private http: HttpClient) {}

  getProjectContext(projectId: string, instanceId: string): Observable<ProjectContext> {
    return this.http.get<WebResponse<ProjectContext>>(`/api/projects/${projectId}/instances/${instanceId}/context`).pipe(map((res) => res.data));
  }

  clearContext(projectId: string, instanceId: string) {
    return this.http.post<void>(`/api/projects/${projectId}/instances/${instanceId}/context/clear`, {});
  }

  reloadContext(projectId: string, instanceId: string) {
    return this.http.post<void>(`/api/projects/${projectId}/instances/${instanceId}/context/reload`, {});
  }
}
