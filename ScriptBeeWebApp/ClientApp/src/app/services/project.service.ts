import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private getProjectUrl = "/api/project/get";

  constructor(private http: HttpClient) { }

  getProject(projectId) {
    return this.http.get(this.getProjectUrl + "/" + projectId);
  }
}
