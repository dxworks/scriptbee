import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileTreeNode } from '../../project-details/scripts-content/fileTreeNode';
import { HttpClient, HttpParams } from '@angular/common/http';
import { contentHeaders } from '../../shared/headers';

@Injectable({
  providedIn: 'root'
})
export class FileSystemService {
  private projectStructureAPIUrl = '/api/projectstructure';
  private projectStructureScriptAPIUrl = '/api/projectstructure/script';
  private projectStructureScriptGetAbsolutePathAPIUrl = '/api/projectstructure/scriptabsolutepath';
  private projectStructureProjectAbsolutePath = '/api/projectstructure/projectabsolutepath';
  private projectStructureFileWatcherUrl = '/api/projectstructure/filewatcher';

  constructor(private http: HttpClient) {}

  getFileSystem(projectId: string): Observable<FileTreeNode> {
    return this.http.get<FileTreeNode>(`${this.projectStructureAPIUrl}/${projectId}`, {
      headers: contentHeaders
    });
  }

  createScript(projectId: string, filePath: string, scriptType: string) {
    return this.http.post<FileTreeNode>(
      this.projectStructureScriptAPIUrl,
      {
        projectId,
        filePath,
        scriptType
      },
      { headers: contentHeaders }
    );
  }

  getFileContent(projectId: string, filePath: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId).set('filePath', filePath);
    return this.http.get<string>(this.projectStructureScriptAPIUrl, {
      headers: contentHeaders,
      params
    });
  }

  getScriptAbsolutePath(projectId: string, filePath: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId).set('filePath', filePath);
    return this.http.get<string>(this.projectStructureScriptGetAbsolutePathAPIUrl, {
      headers: contentHeaders,
      params
    });
  }

  getProjectAbsolutePath(projectId: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.get<string>(this.projectStructureProjectAbsolutePath, {
      headers: contentHeaders,
      params
    });
  }

  addFileWatcher(projectId: string) {
    return this.http.post(this.projectStructureFileWatcherUrl, { projectId }, { headers: contentHeaders });
  }

  removeFileWatcher(projectId: string) {
    return this.http.delete(`${this.projectStructureFileWatcherUrl}/${projectId}`, {
      headers: contentHeaders
    });
  }
}
