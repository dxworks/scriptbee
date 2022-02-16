import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {contentHeaders} from '../../shared/headers';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  private loadersAPIUrl = '/api/loaders';

  constructor(private http: HttpClient) {
  }

  getAllLoaders() {
    return this.http.get<string[]>(this.loadersAPIUrl, {headers: contentHeaders});
  }
}

