import { computed, inject, Injectable, signal } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';
import { ProjectService } from '../projects/project.service';
import { AuthService } from './AuthService';
import { Permission } from '../../types/permissions';
import { GlobalPermissionsService } from '../projects/global-permissions.service';

@Injectable({
  providedIn: 'root',
})
export class PermissionsService {
  private readonly projectId = signal<string>('');
  private readonly projectService = inject(ProjectService);
  private readonly globalPermissionsService = inject(GlobalPermissionsService);
  private readonly authService = inject(AuthService);

  private readonly isAuthenticated = toSignal(this.authService.isAuthenticated$, { initialValue: false });

  private readonly globalPermissionsResource = rxResource({
    params: () => this.isAuthenticated(),
    stream: ({ params: isAuth }) => {
      if (!isAuth) {
        return of([] as string[]);
      }

      return this.globalPermissionsService.getPermissions().pipe(catchError(() => of([] as string[])));
    },
  });

  private readonly projectPermissionsResource = rxResource({
    params: () => {
      const projectId = this.projectId();

      return projectId ? { projectId } : undefined;
    },
    stream: ({ params }) => this.projectService.getPermissions(params.projectId).pipe(catchError(() => of([] as string[]))),
  });

  readonly status = this.projectPermissionsResource.status;
  readonly globalStatus = this.globalPermissionsResource.status;

  readonly globalPermissions = computed<string[]>(() => this.globalPermissionsResource.value() ?? []);
  readonly projectPermissions = computed<string[]>(() => this.projectPermissionsResource.value() ?? []);
  readonly permissions = computed<string[]>(() => [...new Set([...this.globalPermissions(), ...this.projectPermissions()])]);

  setProjectId(projectId: string) {
    this.projectId.set(projectId);
  }

  hasPermission(permission: Permission): boolean {
    return this.permissions().includes(permission);
  }
}
