import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Linker } from '../../types/link-model';

@Injectable({
  providedIn: 'root',
})
export class LinkerService {
  private linkersAPIUrl = '/api/linkers';

  constructor(private http: HttpClient) {}

  linkModels(projectId: string, linkerId: string) {
    return this.http.post(this.linkersAPIUrl, {
      projectId: projectId,
      linkerId: linkerId,
    });
  }

  getAllLinkers(projectId: string, instanceId: string) {
    return this.http.get<Linker[]>(`/api/projects/${projectId}/instances/${instanceId}/loaders`);
  }
}
