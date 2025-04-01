import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Linker } from '../../types/link-model';

@Injectable({
  providedIn: 'root',
})
export class LinkerService {
  constructor(private http: HttpClient) {}

  getAllLinkers(projectId: string, instanceId: string) {
    return this.http.get<Linker[]>(`/api/projects/${projectId}/instances/${instanceId}/loaders`);
  }

  linkModels(projectId: string, instanceId: string, linkerId: string) {
    return this.http.post(`/api/projects/${projectId}/instances/${instanceId}/context/link`, {
      linkerIds: [linkerId],
    });
  }
}
