import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProjectContext } from '../../types/returned-context-slice';

@Injectable({
  providedIn: 'root',
})
export class ProjectContextService {
  private projectsAPIUrl = '/api/projects';

  constructor(private http: HttpClient) {}

  getProjectContext(projectId: string, instanceId: string): Observable<ProjectContext> {
    return this.http.get<ProjectContext>(`${this.projectsAPIUrl}/${projectId}/instances/${instanceId}/context`);
  }

  clearContext(projectId: string, instanceId: string) {
    return this.http.post<void>(`${this.projectsAPIUrl}/${projectId}/instances/${instanceId}/context/clear`, {});
  }

  reloadContext(projectId: string, instanceId: string) {
    return this.http.post<void>(`${this.projectsAPIUrl}/${projectId}/instances/${instanceId}/context/reload`, {});
  }
}
