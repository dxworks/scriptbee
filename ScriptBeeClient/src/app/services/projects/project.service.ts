import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  CreateProjectRequest,
  CreateProjectResponse,
  Project,
  ProjectMember,
  ProjectMembersResponse,
  ProjectPermissionsResponse,
  RoleInfo,
  RolesResponse,
  UserInfo,
  UsersResponse,
} from '../../types/project';
import { map, Observable } from 'rxjs';
import { WebResponse } from '../../types/web-response';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private projectsAPIUrl = '/api/projects';

  private http = inject(HttpClient);

  getAllProjects(): Observable<Project[]> {
    return this.http.get<WebResponse<Project[]>>(this.projectsAPIUrl).pipe(map((r) => r.data));
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

  getPermissions(projectId: string): Observable<string[]> {
    return this.http.get<ProjectPermissionsResponse>(`${this.projectsAPIUrl}/${projectId}/permissions`).pipe(map((r) => r.permissions));
  }

  getProjectMembers(projectId: string): Observable<ProjectMember[]> {
    return this.http.get<ProjectMembersResponse>(`${this.projectsAPIUrl}/${projectId}/members`).pipe(map((r) => r.members));
  }

  updateProjectMember(projectId: string, memberId: string, role: string, memberType: string): Observable<void> {
    return this.http.put<void>(`${this.projectsAPIUrl}/${projectId}/members/${memberId}`, { role, memberType });
  }

  removeProjectMember(projectId: string, memberId: string, memberType: string): Observable<void> {
    return this.http.delete<void>(`${this.projectsAPIUrl}/${projectId}/members/${memberId}?memberType=${memberType}`);
  }

  getAllUsers(): Observable<UserInfo[]> {
    return this.http.get<UsersResponse>('/api/users').pipe(map((r) => r.users));
  }

  getRoles(): Observable<RoleInfo[]> {
    return this.http.get<RolesResponse>('/api/roles').pipe(map((r) => r.roles));
  }
}
