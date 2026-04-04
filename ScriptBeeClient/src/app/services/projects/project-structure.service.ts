import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { GetProjectFilesResponse, ProjectScript } from '../../types/project';
import { CreateScriptRequest, ScriptLanguage, ScriptParameter, UpdateScriptRequest } from '../../types/script-types';
import { WebResponse } from '../../types/web-response';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProjectStructureService {
  constructor(private http: HttpClient) {}

  deleteProjectStructureNode(projectId: string, id: string) {
    return this.http.delete(`/api/projects/${projectId}/files/${id}`);
  }

  getProjectFiles(projectId: string, parentId?: string, offset: number = 0, limit: number = 50) {
    let params = new HttpParams().set('offset', offset).set('limit', limit);
    if (parentId) {
      params = params.set('parentId', parentId);
    }
    return this.http.get<GetProjectFilesResponse>(`/api/projects/${projectId}/files`, { params });
  }

  getAvailableScriptTypes(projectId: string) {
    return this.http.get<WebResponse<ScriptLanguage[]>>(`/api/projects/${projectId}/structure/available-script-types`).pipe(map((res) => res.data));
  }

  getProjectScript(projectId: string, scriptId: string) {
    return this.http.get<ProjectScript>(`/api/projects/${projectId}/scripts/${scriptId}`);
  }

  getScriptContent(projectId: string, scriptId: string) {
    return this.http.get<string>(`/api/projects/${projectId}/scripts/${scriptId}/content`);
  }

  createProjectScript(projectId: string, scriptPath: string, scriptLanguage: string, parameters: ScriptParameter[]) {
    const request: CreateScriptRequest = {
      path: scriptPath,
      language: scriptLanguage,
      parameters: parameters,
    };
    return this.http.post<ProjectScript>(`/api/projects/${projectId}/scripts`, request);
  }

  updateProjectScript(projectId: string, scriptId: string, parameters: ScriptParameter[] | undefined) {
    const request: Partial<UpdateScriptRequest> = {
      parameters: parameters,
    };
    return this.http.patch<ProjectScript>(`/api/projects/${projectId}/scripts/${scriptId}`, request);
  }
}
