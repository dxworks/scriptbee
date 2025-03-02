import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InstanceInfo } from '../../types/instance';

@Injectable({
  providedIn: 'root',
})
export class InstanceService {
  private projectsAPIUrl = '/api/projects';

  constructor(private http: HttpClient) {}

  getCurrentInstance(projectId: string): Observable<InstanceInfo> {
    return this.http.get<InstanceInfo>(`${this.projectsAPIUrl}/${projectId}/instances/current`);
  }
}
