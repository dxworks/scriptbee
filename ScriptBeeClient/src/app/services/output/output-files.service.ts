import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { contentHeaders } from '../../shared/headers';

@Injectable({
  providedIn: 'root'
})
export class OutputFilesService {
  private getOutputDataUrl = '/api/output';
  private downloadFileApiURL = '/api/output/files/download';
  private downloadAllApiURL = '/api/output/files/downloadAll';

  constructor(private http: HttpClient) {}

  downloadFile(id: string, name: string) {
    return this.http.post(
      this.downloadFileApiURL,
      { id, name },
      {
        headers: contentHeaders,
        responseType: 'blob'
      }
    );
  }

  downloadAll(projectId: string, runIndex: number) {
    return this.http.post(
      this.downloadAllApiURL,
      {
        projectId,
        runIndex
      },
      {
        headers: contentHeaders,
        responseType: 'blob'
      }
    );
  }

  fetchOutput(outputId: string) {
    return this.http.get<string>(`${this.getOutputDataUrl}/${outputId}`, {
      headers: contentHeaders
    });
  }
}
