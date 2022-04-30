import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Project} from '../../projects/project';
import {map} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {contentHeaders} from '../../shared/headers';
import {TreeNode} from '../../shared/tree-node';
import {ReturnedProject} from "./returned-project";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private projectsAPIUrl = '/api/projects';

  constructor(private http: HttpClient) {
  }

  getProject(projectId: string): Observable<Project> {
    return this.http.get<ReturnedProject>(`${this.projectsAPIUrl}/${projectId}`, {headers: contentHeaders}).pipe(map((data: ReturnedProject) => {
      return ProjectService.convertReturnedProjectToProject(data);
    }));
  }

  getProjectContext(projectId: string) {
    return this.http.get<TreeNode[]>(`${this.projectsAPIUrl}/context/${projectId}`, {headers: contentHeaders});
  }

  getAllProjects(): Observable<Project[]> {
    return this.http.get<ReturnedProject[]>(this.projectsAPIUrl, {headers: contentHeaders}).pipe(map((data: ReturnedProject[]) => {
      return data.map((project: ReturnedProject) => {
        return ProjectService.convertReturnedProjectToProject(project);
      });
    }));
  }

  createProject(projectId: string, projectName: string) {
    return this.http.post(this.projectsAPIUrl, {
      projectId: projectId,
      projectName: projectName
    }, {headers: contentHeaders});
  }

  deleteProject(projectId: string) {
    return this.http.delete(`${this.projectsAPIUrl}/${projectId}`, {headers: contentHeaders});
  }

  private static convertReturnedProjectToProject(returnedProject: ReturnedProject) {
    const savedFiles: TreeNode[] = returnedProject.savedFiles.map(file => {
      return {
        name: file.loaderName,
        children: file.files.map(f => ({
          name: f
        }))
      }
    });

    const loadedFiles: TreeNode[] = returnedProject.loadedFiles.map(file => {
      return {
        name: file.loaderName,
        children: file.files.map(f => ({
          name: f
        }))
      }
    });

    return ({
      projectId: returnedProject.id,
      projectName: returnedProject.name,
      creationDate: returnedProject.creationDate,
      linker: returnedProject.linker,
      loaders: returnedProject.loaders,
      savedFiles: savedFiles,
      loadedFiles: loadedFiles
    });
  }
}
