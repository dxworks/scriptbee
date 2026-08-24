import { inject, Injectable, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProjectService } from '../projects/project.service';

@Injectable({
  providedIn: 'root',
})
export class PermissionsService {
  private readonly projectId = signal<string>('');
  private readonly projectService = inject(ProjectService);

  private readonly projectPermissionsResource = rxResource({
    params: () => {
      const projectId = this.projectId();

      return projectId ? { projectId } : undefined;
    },
    stream: ({ params }) => this.projectService.getPermissions(params.projectId),
  });
  setProjectId(projectId: string) {
    this.projectId.set(projectId);
  }

  hasPermission(permission: string) {
    return this.projectPermissionsResource.value()?.includes(permission) ?? false;
  }
}
