import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AnalysisConsoleOutput, AnalysisFileOutput, AnalysisRunErrorOutput } from '../../types/analysis-results';

@Injectable({
  providedIn: 'root',
})
export class OutputFilesService {
  constructor(private http: HttpClient) {}

  getConsoleOutput(projectId: string, analysisId: string) {
    return this.http.get<AnalysisConsoleOutput>(`/api/projects/${projectId}/analyses/${analysisId}/results/console`);
  }

  getErrorOutputs(projectId: string, analysisId: string) {
    return this.http.get<AnalysisRunErrorOutput>(`/api/projects/${projectId}/analyses/${analysisId}/results/errors`);
  }

  getFileOutputs(projectId: string, analysisId: string) {
    return this.http.get<AnalysisFileOutput>(`/api/projects/${projectId}/analyses/${analysisId}/results/files`);
  }

  downloadFile(id: string, name: string) {
    return this.http.post(
      '/api/output/files/download',
      { id, name },
      {
        responseType: 'blob',
      }
    );
  }

  downloadAll(projectId: string, runIndex: number) {
    return this.http.post(
      '/api/output/files/downloadAll',
      {
        projectId,
        runIndex,
      },
      {
        responseType: 'blob',
      }
    );
  }

  fetchOutput(outputId: string) {
    return this.http.get<string>(`/api/output/${outputId}`);
  }
}
