import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProjectScript, ProjectStructureNode } from '../../types/project';
import { CreateScriptRequest, ScriptLanguage, ScriptParameter } from '../../types/script-types';

@Injectable({
  providedIn: 'root',
})
export class ProjectStructureService {
  constructor(private http: HttpClient) {}

  getProjectStructure(projectId: string) {
    return this.http.get<ProjectStructureNode[]>(`/api/projects/${projectId}/structure`);
  }

  getAvailableScriptTypes(projectId: string) {
    return this.http.get<ScriptLanguage[]>(`/api/projects/${projectId}/structure/available-script-types`);
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
}
