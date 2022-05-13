import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {contentHeaders} from '../../shared/headers';

@Injectable({
  providedIn: 'root'
})
export class LinkerService {

  private linkersAPIUrl = '/api/linkers';

  constructor(private http: HttpClient) {
  }

  linkModels(projectId: string, linkerName: string) {
    return this.http.post(this.linkersAPIUrl, {
      projectId: projectId,
      linkerName: linkerName
    }, {headers: contentHeaders});
  }

  getAllLinkers() {
    return this.http.get<string[]>(this.linkersAPIUrl, {headers: contentHeaders})
  }
}
