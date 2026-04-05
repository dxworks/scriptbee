import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Linker } from '../../types/link-model';
import { WebResponse } from '../../types/web-response';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LinkerService {
  private http = inject(HttpClient);

  getAllLinkers(projectId: string, instanceId: string) {
    return this.http.get<WebResponse<Linker[]>>(`/api/projects/${projectId}/instances/${instanceId}/loaders`).pipe(map((res) => res.data));
  }

  linkModels(projectId: string, instanceId: string, linkerId: string) {
    return this.http.post(`/api/projects/${projectId}/instances/${instanceId}/context/link`, {
      linkerIds: [linkerId],
    });
  }
}
