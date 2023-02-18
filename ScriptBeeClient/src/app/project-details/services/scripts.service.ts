import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ScriptLanguage } from './create-script';
import { contentHeaders } from '../../shared/headers';

@Injectable({
  providedIn: 'root',
})
export class ScriptsService {
  private scriptLanguagesApi = '/api/scripts/languages';
  private createScriptApi = '/api/create-script';

  constructor(private http: HttpClient) {}

  getAvailableLanguages(): Observable<ScriptLanguage[]> {
    return this.http.get<ScriptLanguage[]>(this.scriptLanguagesApi, { headers: contentHeaders });
  }
}
