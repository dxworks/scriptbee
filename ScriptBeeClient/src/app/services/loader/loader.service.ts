import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {contentHeaders} from '../../shared/headers';
import {TreeNode} from '../../shared/tree-node';
import {LoadModel} from './load-model';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  private loadersAPIUrl = '/api/loaders';
  private loadersClearContextAPIUrl = '/api/loaders/clear';

  constructor(private http: HttpClient) {
  }

  getAllLoaders() {
    return this.http.get<string[]>(this.loadersAPIUrl, {headers: contentHeaders});
  }

  loadModels(projectId: string, checkedFiles: TreeNode[]) {
    const loadModels: LoadModel = {
      projectId: projectId,
      nodes: checkedFiles.map(treeNode => ({
        loaderName: treeNode.name,
        models: treeNode.children.map(child => child.name)
      }))
    };
    return this.http.post(this.loadersAPIUrl, loadModels, {headers: contentHeaders});
  }

  reloadProjectContext(projectId: string) {
    return this.http.post(`${this.loadersAPIUrl}/${projectId}`, {headers: contentHeaders});
  }

  clearProjectContext(projectId: string) {
    return this.http.post(`${this.loadersClearContextAPIUrl}/${projectId}`, {headers: contentHeaders});
  }
}

