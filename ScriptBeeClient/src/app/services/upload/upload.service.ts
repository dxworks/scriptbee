import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UploadModelsResult } from '../../types/upload-models-result';

@Injectable({
  providedIn: 'root',
})
export class UploadService {
  private uploadFilesUrl = '/api/uploadmodel/fromfile';

  constructor(private http: HttpClient) {}

  uploadModels(loaderId: string, projectId: string, files: File[]) {
    const formData = new FormData();
    formData.append('loaderId', loaderId);
    files.forEach((file) => formData.append('files', file));
    formData.append('projectId', projectId);

    return this.http.post<UploadModelsResult>(this.uploadFilesUrl, formData);
  }
}
