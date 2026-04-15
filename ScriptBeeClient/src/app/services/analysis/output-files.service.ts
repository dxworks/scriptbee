import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AnalysisConsoleOutput, AnalysisFileOutput, AnalysisRunErrorOutput } from '../../types/analysis-results';

@Injectable({
  providedIn: 'root',
})
export class OutputFilesService {
  private http = inject(HttpClient);

  getConsoleOutput(projectId: string, analysisId: string) {
    return this.http.get<AnalysisConsoleOutput>(`/api/projects/${projectId}/analyses/${analysisId}/results/console`);
  }

  getErrorOutputs(projectId: string, analysisId: string) {
    return this.http.get<AnalysisRunErrorOutput>(`/api/projects/${projectId}/analyses/${analysisId}/results/errors`);
  }

  getFileOutputs(projectId: string, analysisId: string) {
    return this.http.get<AnalysisFileOutput>(`/api/projects/${projectId}/analyses/${analysisId}/results/files`);
  }

  downloadFile(projectId: string, analysisId: string, fileId: string) {
    return this.http.get(`/api/projects/${projectId}/analyses/${analysisId}/results/files/${fileId}`, {
      responseType: 'blob',
    });
  }

  downloadAll(projectId: string, analysisId: string) {
    return this.http.get(`/api/projects/${projectId}/analyses/${analysisId}/results/files/download`, {
      responseType: 'blob',
    });
  }

  fetchOutput(outputId: string) {
    return this.http.get<string>(`/api/output/${outputId}`);
  }
}
