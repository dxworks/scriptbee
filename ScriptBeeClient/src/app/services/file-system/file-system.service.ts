import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {FileTreeNode} from '../../project-details/scripts-content/fileTreeNode';
import {HttpClient, HttpParams} from "@angular/common/http";
import {contentHeaders} from "../../shared/headers";

interface ScriptCreatedResult {
  filePath: string;
}

@Injectable({
  providedIn: 'root'
})
export class FileSystemService {

  private projectStructureAPIUrl = '/api/projectstructure';
  private projectStructureScriptAPIUrl = '/api/projectstructure/script';
  private projectStructureScriptGetAbsolutePathAPIUrl = '/api/projectstructure/absolutepath';

  constructor(private http: HttpClient) {
  }

  getFileSystem(projectId: string): Observable<FileTreeNode> {
    return this.http.get<FileTreeNode>(`${this.projectStructureAPIUrl}/${projectId}`, {headers: contentHeaders});
  }

  createScript(projectId: string, filePath: string, scriptType: string) {
    return this.http.post<ScriptCreatedResult>(this.projectStructureScriptAPIUrl, {
      projectId: projectId,
      filePath: filePath,
      scriptType: scriptType
    }, {headers: contentHeaders});
  }

  getFileContent(projectId: string, filePath: string): Observable<string> {
    const params = new HttpParams().set("projectId", projectId).set("filePath", filePath);
    return this.http.get<string>(this.projectStructureScriptAPIUrl, {headers: contentHeaders, params: params});
  }

  getScriptAbsolutePath(projectId: string, filePath: string): Observable<string> {
    const params = new HttpParams().set("projectId", projectId).set("filePath", filePath);
    return this.http.get<string>(this.projectStructureScriptGetAbsolutePathAPIUrl, {
      headers: contentHeaders,
      params: params
    });
  }
}
