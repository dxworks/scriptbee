import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TreeNodeWithParent } from '../../types/tree-node';
import { Loader } from '../../types/load-model';
import { ReturnedContextSlice } from '../../types/returned-context-slice';
import { WebResponse } from '../../types/web-response';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  constructor(private http: HttpClient) {}

  getAllLoaders(projectId: string, instanceId: string) {
    return this.http.get<WebResponse<Loader[]>>(`/api/projects/${projectId}/instances/${instanceId}/loaders`).pipe(map((res) => res.data));
  }

  loadModels(projectId: string, instanceId: string, checkedFiles: TreeNodeWithParent[]) {
    const loaderIds = checkedFiles.map((node) => node.parent?.name);

    return this.http.post<ReturnedContextSlice[]>(`/api/projects/${projectId}/instances/${instanceId}/context/load`, {
      loaderIds,
    });
  }
}
