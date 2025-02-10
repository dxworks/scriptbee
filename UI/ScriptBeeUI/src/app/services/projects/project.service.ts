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

  // getProjectContext(projectId: string): Observable<TreeNode[]> {
  //   return this.http.get<ReturnedContextSlice[]>(`${this.projectsAPIUrl}/context/${projectId}`, { headers: contentHeaders }).pipe(
  //     map((data: ReturnedContextSlice[]) => {
  //       return ProjectService.convertReturnedContextSlicesToContext(data);
  //     })
  //   );
  // }

  //
  // private static convertReturnedProjectToProject(returnedProject: ReturnedProject): ProjectData {
  //   function mapFiles(file: ReturnedNode) {
  //     return {
  //       name: file.loaderName,
  //       children: file.files.map((f) => ({
  //         name: f,
  //       })),
  //     };
  //   }
  //
  //   const savedFiles: TreeNode[] = returnedProject.savedFiles.map((file) => mapFiles(file));
  //   const loadedFiles: TreeNode[] = returnedProject.loadedFiles.map((file) => mapFiles(file));
  //
  //   return {
  //     projectId: returnedProject.id,
  //     projectName: returnedProject.name,
  //     creationDate: returnedProject.creationDate,
  //     linker: returnedProject.linker,
  //     loaders: returnedProject.loaders,
  //     savedFiles: savedFiles,
  //     loadedFiles: loadedFiles,
  //   };
  // }
  //
  // private static convertReturnedContextSlicesToContext(contextSlices: ReturnedContextSlice[]) {
  //   return contextSlices.map((slice) => ({
  //     name: slice.name,
  //     children: slice.models.map((model) => ({
  //       name: model,
  //     })),
  //   }));
  // }
}
