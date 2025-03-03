import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProjectStructureNode } from '../../types/project';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProjectStructureService {
  constructor(private http: HttpClient) {}

  getProjectStructure(projectId: string) {
    return this.http.get<ProjectStructureNode[]>(`/api/projects/${projectId}/structure`);
  }

  getScriptContent(projectId: string, scriptId: string) {
    return this.http.get<string>(`/api/projects/${projectId}/structure`, {
      params: {
        scriptId,
      },
    });
  }
}
