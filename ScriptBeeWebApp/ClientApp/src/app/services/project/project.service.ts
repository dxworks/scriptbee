import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Project} from '../../projects/project';
import {map} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {contentHeaders} from '../../shared/headers';
import {TreeNode} from "../../shared/tree/tree.component";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private projectsAPIUrl = '/api/projects';

  constructor(private http: HttpClient) {
  }

  getProject(projectId: string): Observable<Project> {
    return this.http.get(`${this.projectsAPIUrl}/${projectId}`, {headers: contentHeaders}).pipe(map((data: any) => ({
      projectId: data.id,
      projectName: data.name,
      creationDate: data.creationDate
    })));
  }

  getProjectContext(projectId: string) {
    return this.http.get<TreeNode[]>(`${this.projectsAPIUrl}/context/${projectId}`, {headers: contentHeaders});
  }

  getAllProjects(): Observable<Project[]> {
    return this.http.get(this.projectsAPIUrl, {headers: contentHeaders}).pipe(map((data: any[]) => {
      return data.map((project: any) => ({
        projectId: project.id,
        projectName: project.name,
        creationDate: project.creationDate
      }));
    }));
  }

  createProject(projectName: string) {
    return this.http.post(this.projectsAPIUrl, {projectName: projectName}, {headers: contentHeaders});
  }

  deleteProject(projectId: string) {
    return this.http.delete(`${this.projectsAPIUrl}/${projectId}`, {headers: contentHeaders});
  }
}
