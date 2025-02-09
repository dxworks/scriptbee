import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProjectListResponse } from '../../types/project';
import { rxResource } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private projectsAPIUrl = '/api/projects';

  constructor(private http: HttpClient) {}

  projectsResource = rxResource({
    loader: () => this.http.get<ProjectListResponse>(this.projectsAPIUrl).pipe(map((r) => r.projects)),
  });

  // getAllProjects(): Observable<ProjectData[]> {
  //   return this.http.get<ReturnedProject[]>(this.projectsAPIUrl, { headers: contentHeaders }).pipe(
  //     map((data: ReturnedProject[]) => {
  //       return data.map((project: ReturnedProject) => {
  //         return ProjectService.convertReturnedProjectToProject(project);
  //       });
  //     })
  //   );
  // }

  // getProject(projectId: string): Observable<ProjectData> {
  //   return this.http.get<ReturnedProject>(`${this.projectsAPIUrl}/${projectId}`, { headers: contentHeaders }).pipe(
  //     map((data: ReturnedProject) => {
  //       return ProjectService.convertReturnedProjectToProject(data);
  //     })
  //   );
  // }

  // getProjectContext(projectId: string): Observable<TreeNode[]> {
  //   return this.http.get<ReturnedContextSlice[]>(`${this.projectsAPIUrl}/context/${projectId}`, { headers: contentHeaders }).pipe(
  //     map((data: ReturnedContextSlice[]) => {
  //       return ProjectService.convertReturnedContextSlicesToContext(data);
  //     })
  //   );
  // }

  // getAllProjects(): Observable<ProjectData[]> {
  //   return this.http.get<ReturnedProject[]>(this.projectsAPIUrl, { headers: contentHeaders }).pipe(
  //     map((data: ReturnedProject[]) => {
  //       return data.map((project: ReturnedProject) => {
  //         return ProjectService.convertReturnedProjectToProject(project);
  //       });
  //     })
  //   );
  // }

  // createProject(projectId: string, projectName: string) {
  //   return this.http.post(
  //     this.projectsAPIUrl,
  //     {
  //       projectId: projectId,
  //       projectName: projectName,
  //     },
  //     { headers: contentHeaders }
  //   );
  // }
  //
  // deleteProject(projectId: string) {
  //   return this.http.delete(`${this.projectsAPIUrl}/${projectId}`, { headers: contentHeaders });
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
