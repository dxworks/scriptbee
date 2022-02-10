import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Project} from "../../projects/project";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private getProjectUrl = "/api/project/get";
  private getAllProjectsUrl = "/api/project/getAll";

  // private createProjectUrl = "";

  constructor(private http: HttpClient) {
  }

  getProject(projectId) {
    return this.http.get<Project>(this.getProjectUrl + "/" + projectId);
  }

  getAllProjects() {
    return this.http.get<Project[]>(this.getAllProjectsUrl);
  }

  // createProject(projectId, projectName)
}
