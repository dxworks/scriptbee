import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateProjectRequest, CreateProjectResponse, Project, ProjectListResponse } from '../../types/project';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private projectsAPIUrl = '/api/projects';

  constructor(private http: HttpClient) {}

  getAllProjects(): Observable<Project[]> {
    return this.http.get<ProjectListResponse>(this.projectsAPIUrl).pipe(map((r) => r.projects));
  }

  createProject(projectId: string, projectName: string) {
    const body: CreateProjectRequest = {
      id: projectId,
      name: projectName,
    };
    return this.http.post<CreateProjectResponse>(this.projectsAPIUrl, body);
  }

  getProject(projectId: string): Observable<Project> {
    return this.http.get<Project>(`${this.projectsAPIUrl}/${projectId}`);
  }

  deleteProject(projectId: string) {
    return this.http.delete(`${this.projectsAPIUrl}/${projectId}`);
  }
}
