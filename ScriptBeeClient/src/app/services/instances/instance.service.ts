import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { InstanceInfo } from '../../types/instance';
import { WebResponse } from '../../types/web-response';

@Injectable({
  providedIn: 'root',
})
export class InstanceService {
  private http = inject(HttpClient);

  allocateInstance(projectId: string): Observable<string> {
    return this.http
      .post<InstanceInfo>(
        `/api/projects/${projectId}/instances`,
        null,

        {
          observe: 'response',
        }
      )
      .pipe(map((response) => response.headers.get('Location') ?? ''));
  }

  getInstanceStatus(url: string): Observable<InstanceInfo> {
    return this.http.get<InstanceInfo>(url);
  }

  getProjectInstances(projectId: string): Observable<InstanceInfo[]> {
    return this.http.get<WebResponse<InstanceInfo[]>>(`/api/projects/${projectId}/instances`).pipe(map((r) => r.data));
  }

  deallocateInstance(projectId: string, instanceId: string): Observable<void> {
    return this.http.delete<void>(`/api/projects/${projectId}/instances/${instanceId}`);
  }
}
