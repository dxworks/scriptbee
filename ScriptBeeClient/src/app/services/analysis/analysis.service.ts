import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { AnalysisStatus } from '../../types/analysis-status';
import { WebResponse } from '../../types/web-response';

@Injectable({
  providedIn: 'root',
})
export class AnalysisService {
  private http = inject(HttpClient);

  triggerAnalysis(projectId: string, instanceId: string, scriptId: string): Observable<string> {
    return this.http
      .post<void>(
        `/api/projects/${projectId}/instances/${instanceId}/analyses`,
        {
          scriptId,
        },
        {
          observe: 'response',
        }
      )
      .pipe(map((response) => response.headers.get('Location') ?? ''));
  }

  getAnalysisStatus(url: string): Observable<AnalysisStatus> {
    return this.http.get<AnalysisStatus>(url);
  }

  getAnalyses(projectId: string): Observable<AnalysisStatus[]> {
    return this.http.get<WebResponse<AnalysisStatus[]>>(`/api/projects/${projectId}/analyses`).pipe(map((res) => res.data));
  }
}
