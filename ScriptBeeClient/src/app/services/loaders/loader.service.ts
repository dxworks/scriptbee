import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TreeNodeWithParent } from '../../types/tree-node';
import { Loader } from '../../types/load-model';
import { ReturnedContextSlice } from '../../types/returned-context-slice';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  constructor(private http: HttpClient) {}

  getAllLoaders(projectId: string, instanceId: string) {
    return this.http.get<Loader[]>(`/api/projects/${projectId}/instances/${instanceId}/loaders`);
  }

  loadModels(projectId: string, instanceId: string, checkedFiles: TreeNodeWithParent[]) {
    const loaderIds = checkedFiles.map((node) => node.parent?.name);

    return this.http.post<ReturnedContextSlice[]>(`/api/projects/${projectId}/instances/${instanceId}/context/load`, {
      loaderIds,
    });
  }
}
