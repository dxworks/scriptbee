import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { contentHeaders } from '../../shared/headers';
import { FileTreeNode } from '../components/run-script/scripts-content/fileTreeNode';
import { CreateScriptData, ScriptLanguage } from './script-types';

@Injectable({
  providedIn: 'root',
})
export class ScriptsService {
  private scriptLanguagesApi = '/api/scripts/languages';
  private createScriptApi = '/api/scripts';

  constructor(private http: HttpClient) {}

  getAvailableLanguages(): Observable<ScriptLanguage[]> {
    return this.http.get<ScriptLanguage[]>(this.scriptLanguagesApi, { headers: contentHeaders });
  }

  createScript(createScriptData: CreateScriptData) {
    return this.http.post<FileTreeNode>(this.createScriptApi, createScriptData, { headers: contentHeaders });
  }
}
