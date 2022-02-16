import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UploadService {

  private uploadFilesUrl = '/api/uploadmodel/fromfile';

  constructor(private http: HttpClient) {
  }

  uploadModels(loaderName: string, projectId: string, files: any) {
    const formData = new FormData();
    formData.append("loaderName", loaderName);
    formData.append("files", files);
    formData.append("projectId", projectId);

    return this.http.post(this.uploadFilesUrl, formData);
  }
}
