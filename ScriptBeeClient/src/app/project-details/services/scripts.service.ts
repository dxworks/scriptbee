import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { contentHeaders } from '../../shared/headers';
import {
  CreateScriptData,
  CreateScriptResponse,
  ScriptData,
  ScriptFileStructureNode,
  ScriptLanguage,
  UpdateScriptData,
  UpdateScriptResponse,
} from './script-types';

@Injectable({
  providedIn: 'root',
})
export class ScriptsService {
  private scriptLanguagesApi = '/api/scripts/languages';
  private scriptsApi = '/api/scripts';

  constructor(private http: HttpClient) {}

  getAvailableLanguages(): Observable<ScriptLanguage[]> {
    return this.http.get<ScriptLanguage[]>(this.scriptLanguagesApi, { headers: contentHeaders });
  }

  getScripts(projectId: string) {
    return this.http.get<ScriptFileStructureNode[]>(this.scriptsApi, { headers: contentHeaders, params: { projectId } });
  }

  getScriptById(scriptId: string, projectId: string) {
    return this.http.get<ScriptData>(`${this.scriptsApi}/${scriptId}`, { headers: contentHeaders, params: { projectId } });
  }

  getScriptContent(scriptId: string, projectId: string) {
    return this.http.get<string>(`${this.scriptsApi}/${scriptId}/content`, {
      headers: contentHeaders,
      params: { projectId },
    });
  }

  createScript(createScriptData: CreateScriptData) {
    return this.http.post<CreateScriptResponse>(this.scriptsApi, createScriptData, { headers: contentHeaders });
  }

  updateScript(updateScriptData: UpdateScriptData) {
    return this.http.put<UpdateScriptResponse>(this.scriptsApi, updateScriptData, { headers: contentHeaders });
  }

  deleteScript(scriptId: string, projectId: string) {
    return this.http.delete(`${this.scriptsApi}/${scriptId}`, { headers: contentHeaders, params: { projectId } });
  }
}
