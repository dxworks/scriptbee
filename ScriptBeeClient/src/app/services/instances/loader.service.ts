import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Loader } from '../../types/load-model';
import { WebResponse } from '../../types/web-response';
import { map, retry } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  private http = inject(HttpClient);

  getAllLoaders(projectId: string, instanceId: string) {
    return this.http.get<WebResponse<Loader[]>>(`/api/projects/${projectId}/instances/${instanceId}/loaders`).pipe(
      retry({ count: 3, delay: 1000 }),
      map((res) => res.data)
    );
  }

  loadModels(projectId: string, instanceId: string, filesToLoad: Record<string, string[]>) {
    return this.http.post<void>(`/api/projects/${projectId}/instances/${instanceId}/context/load`, {
      filesToLoad,
    });
  }

  unloadModelFile(projectId: string, instanceId: string, loaderId: string, fileId: string) {
    return this.http.delete<void>(`/api/projects/${projectId}/instances/${instanceId}/context/loaders/${loaderId}/files/${fileId}`);
  }

  unloadLoader(projectId: string, instanceId: string, loaderId: string) {
    return this.http.delete<void>(`/api/projects/${projectId}/instances/${instanceId}/context/loaders/${loaderId}`);
  }
}
