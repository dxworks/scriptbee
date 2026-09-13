import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UploadModelsResult } from '../../types/upload-models-result';
import { ProjectFile } from '../../types/project';

@Injectable({
  providedIn: 'root',
})
export class UploadService {
  private http = inject(HttpClient);

  uploadFiles(projectId: string, files: File[]) {
    const formData = new FormData();
    files.forEach((file) => formData.append('files', file));

    return this.http.post<{ files: ProjectFile[] }>(`/api/projects/${projectId}/saved-files`, formData);
  }

  deleteSavedFile(projectId: string, fileId: string) {
    return this.http.delete<void>(`/api/projects/${projectId}/saved-files/${fileId}`);
  }

  uploadModels(projectId: string, loaderId: string, files: File[]) {
    const formData = new FormData();
    files.forEach((file) => formData.append('files', file));

    return this.http.put<UploadModelsResult>(`/api/projects/${projectId}/loaders/${loaderId}/files`, formData);
  }
}
