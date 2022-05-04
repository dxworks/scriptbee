import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {contentHeaders} from '../../shared/headers';
import {RunScriptResult} from './run-script-result';

@Injectable({
  providedIn: 'root'
})
export class RunScriptService {

  private runScriptAPIUrl = '/api/runscript';

  constructor(private http: HttpClient) {
  }

  runScriptFromPath(projectId: string, filePath: string) {
    return this.http.post<RunScriptResult>(this.runScriptAPIUrl, {
      projectId: projectId,
      filePath: filePath
    }, {headers: contentHeaders});
  }
}
