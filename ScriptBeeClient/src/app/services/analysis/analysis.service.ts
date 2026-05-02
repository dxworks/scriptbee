import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { AnalysisStatus } from '../../types/analysis-status';
import { WebResponse } from '../../types/web-response';
import { ProjectScript } from '../../types/project';

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

  getAnalysis(projectId: string, analysisId: string): Observable<AnalysisStatus> {
    return this.http.get<AnalysisStatus>(`/api/projects/${projectId}/analyses/${analysisId}`);
  }

  getAnalysisScriptContent(projectId: string, analysisId: string, scriptId: string): Observable<string> {
    return this.http.get(`/api/projects/${projectId}/analyses/${analysisId}/scripts/${scriptId}/content`, { responseType: 'text' });
  }

  getAnalysisScriptMetadata(projectId: string, analysisId: string, scriptId: string): Observable<ProjectScript> {
    return this.http.get<ProjectScript>(`/api/projects/${projectId}/analyses/${analysisId}/scripts/${scriptId}`);
  }

  deleteAnalysis(projectId: string, analysisId: string): Observable<void> {
    return this.http.delete<void>(`/api/projects/${projectId}/analyses/${analysisId}`);
  }
}
