import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InstanceInfo } from '../../types/instance';

@Injectable({
  providedIn: 'root',
})
export class InstanceService {
  constructor(private http: HttpClient) {}

  getCurrentInstance(projectId: string): Observable<InstanceInfo> {
    return this.http.get<InstanceInfo>(`/api/projects/${projectId}/instances/current`);
  }

  allocateInstance(projectId: string): Observable<InstanceInfo> {
    return this.http.post<InstanceInfo>(`/api/projects/${projectId}/instances`, null);
  }
}
