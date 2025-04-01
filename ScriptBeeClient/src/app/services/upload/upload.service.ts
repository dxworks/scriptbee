import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UploadModelsResult } from '../../types/upload-models-result';

@Injectable({
  providedIn: 'root',
})
export class UploadService {
  constructor(private http: HttpClient) {}

  uploadModels(projectId: string, loaderId: string, files: File[]) {
    const formData = new FormData();
    files.forEach((file) => formData.append('files', file));

    return this.http.put<UploadModelsResult>(`/api/projects/${projectId}/loaders/${loaderId}/files`, formData);
  }
}
