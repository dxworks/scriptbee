import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TreeNode } from '../../types/tree-node';
import { Loader, LoadModel } from '../../types/load-model';
import { ReturnedContextSlice } from '../../types/returned-context-slice';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  private loadersAPIUrl = '/api/loaders';
  private loadersClearContextAPIUrl = '/api/loaders/clear';

  constructor(private http: HttpClient) {}

  getAllLoaders(projectId: string, instanceId: string) {
    return this.http.get<Loader[]>(`/api/projects/${projectId}/instances/${instanceId}/loaders`);
  }

  loadModels(projectId: string, checkedFiles: TreeNode[]) {
    const loadModels: LoadModel = {
      projectId: projectId,
      nodes: checkedFiles
        .filter((treeNode) => treeNode.children && treeNode.children.length > 0)
        .map((treeNode) => ({
          loaderName: treeNode.name,
          models: (treeNode.children ?? []).map((child: any) => child.name),
        })),
    };

    return this.http.post<ReturnedContextSlice[]>(this.loadersAPIUrl, loadModels);
  }

  reloadProjectContext(projectId: string) {
    return this.http.post<ReturnedContextSlice[]>(`${this.loadersAPIUrl}/${projectId}`, null);
  }

  clearProjectContext(projectId: string) {
    return this.http.post(`${this.loadersClearContextAPIUrl}/${projectId}`, null);
  }
}
