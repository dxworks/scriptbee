import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {contentHeaders} from '../../shared/headers';

@Injectable({
  providedIn: 'root'
})
export class OutputFilesService {

  private getConsoleOutputApiURL = '/api/output/console';
  private downloadFileApiURL = '/api/output/files/download';
  private downloadAllApiURL = '/api/output/files/downloadAll';


  constructor(private http: HttpClient) {
  }

  getConsoleOutputContent(consoleOutputPath: string) {
    const params = new HttpParams()
      .append('consoleOutputPath', consoleOutputPath);

    return this.http.get<string>(this.getConsoleOutputApiURL, {
      headers: contentHeaders,
      params: params
    });
  }

  downloadFile(filePath: string) {
    return this.http.post(this.downloadFileApiURL, {filePath: filePath}, {
      headers: contentHeaders,
      responseType: 'blob'
    });
  }

  downloadAll(projectId: string, runId: string) {
    return this.http.post(this.downloadAllApiURL, {
      projectId: projectId,
      runId: runId
    }, {
      headers: contentHeaders,
      responseType: 'blob'
    });
  }
}
