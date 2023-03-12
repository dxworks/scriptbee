import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {FileTreeNode} from '../../project-details/components/run-script/scripts-content/fileTreeNode';
import {HttpClient, HttpParams} from '@angular/common/http';
import {contentHeaders} from '../../shared/headers';

@Injectable({
  providedIn: 'root',
})
export class FileSystemService {
  private projectStructureAPIUrl = '/api/projectstructure';
  private projectStructureProjectAbsolutePath = '/api/projectstructure/projectabsolutepath';
  private projectStructureFileWatcherUrl = '/api/projectstructure/filewatcher';

  constructor(private http: HttpClient) {}

  getFileSystem(projectId: string): Observable<FileTreeNode> {
    return this.http.get<FileTreeNode>(`${this.projectStructureAPIUrl}/${projectId}`, {
      headers: contentHeaders,
    });
  }

  getProjectAbsolutePath(projectId: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.get<string>(this.projectStructureProjectAbsolutePath, {
      headers: contentHeaders,
      params,
    });
  }

  addFileWatcher(projectId: string) {
    return this.http.post(this.projectStructureFileWatcherUrl, { projectId }, { headers: contentHeaders });
  }

  removeFileWatcher(projectId: string) {
    return this.http.delete(`${this.projectStructureFileWatcherUrl}/${projectId}`, {
      headers: contentHeaders,
    });
  }
}
