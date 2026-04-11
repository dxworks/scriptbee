import { inject, Injectable } from '@angular/core';
import { map, of, tap } from 'rxjs';
import { InstanceInfo } from '../../types/instance';
import { InstanceService } from './instance.service';
import { ProjectStateService } from '../projects/project-state.service';
import { rxResource } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class InstanceAllocationService {
  private instanceService = inject(InstanceService);
  private projectStateService = inject(ProjectStateService);

  instancesResource = rxResource({
    params: () => ({
      projectId: this.projectStateService.currentProjectId(),
    }),
    stream: ({ params }) => {
      if (params.projectId) {
        return this.instanceService.getProjectInstances(params.projectId);
      }
      return of([]);
    },
  });

  currentInstanceResource = rxResource({
    params: () => ({
      projectId: this.projectStateService.currentProjectId(),
      currentInstanceId: this.projectStateService.currentInstanceId(),
    }),
    stream: ({ params: { projectId, currentInstanceId } }) => {
      if (!projectId) {
        return of(undefined);
      }
      return this.instanceService.getProjectInstances(projectId).pipe(
        map((instances) => {
          return instances.find((i) => i.id === currentInstanceId);
        })
      );
    },
  });

  public allocateInstance(projectId: string) {
    return this.instanceService.allocateInstance(projectId).pipe(
      tap((instance) => {
        this.instancesResource.reload();
        this.setCurrentInstance(instance);
      })
    );
  }

  public deallocateInstance(projectId: string, instanceId: string) {
    return this.instanceService.deallocateInstance(projectId, instanceId).pipe(
      tap(() => {
        this.instancesResource.reload();
        this.clearCurrentInstance();
      })
    );
  }

  public setCurrentInstance(instance: InstanceInfo) {
    this.projectStateService.currentInstanceId.set(instance.id);
  }

  public clearCurrentInstance() {
    this.projectStateService.currentInstanceId.set(null);
  }

  public reload() {
    this.instancesResource.reload();
  }
}
