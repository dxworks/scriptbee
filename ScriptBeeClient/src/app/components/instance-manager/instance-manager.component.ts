import { Component, computed, effect, inject, output, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { rxResource } from '@angular/core/rxjs-interop';
import { of } from 'rxjs';
import { InstanceService } from '../../services/instances/instance.service';
import { ProjectStateService } from '../../services/projects/project-state.service';
import { CalculationInstanceStatus, InstanceInfo } from '../../types/instance';
import { ConfirmationDialogComponent } from '../dialogs/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-instance-manager',
  imports: [CommonModule, MatButtonModule, MatIconModule, MatMenuModule, MatTooltipModule, MatDividerModule, MatDialogModule, DatePipe],
  templateUrl: './instance-manager.component.html',
  styleUrls: ['./instance-manager.component.scss'],
})
export class InstanceManagerComponent {
  instanceAllocated = output<InstanceInfo>();

  private instanceService = inject(InstanceService);
  private projectStateService = inject(ProjectStateService);
  private dialog = inject(MatDialog);

  constructor() {
    effect(() => {
      const list = this.instances();
      if (list.length > 0 && !this.currentInstanceId()) {
        this.onSelectInstance(list[0]);
      }
    });
  }

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

  instances = computed(() => this.instancesResource.value() ?? []);

  currentInstanceId = computed(() => this.projectStateService.currentInstanceId());

  currentInstance = computed(() => {
    const id = this.currentInstanceId();
    return this.instances().find((i) => i.id === id);
  });

  getStatusIcon(status: CalculationInstanceStatus): string {
    switch (status) {
      case 'Running':
        return 'check_circle';
      case 'Allocating':
        return 'pending';
      case 'Deallocating':
        return 'delete_sweep';
      case 'NotFound':
        return 'error';
      default:
        return 'help_outline';
    }
  }

  getStatusClass(status: CalculationInstanceStatus): string {
    return status.toLowerCase();
  }

  onSelectInstance(instance: InstanceInfo) {
    this.projectStateService.currentInstanceId.set(instance.id);
  }

  onDeallocateInstance(event: MouseEvent, instance: InstanceInfo) {
    event.stopPropagation();
    const projectId = this.projectStateService.currentProjectId();
    if (projectId) {
      this.dialog
        .open(ConfirmationDialogComponent, {
          data: {
            title: 'Deallocate Instance',
            message: `Are you sure you want to deallocate instance ${instance.id.substring(0, 8)}?`,
            confirmText: 'Deallocate',
          },
        })
        .afterClosed()
        .subscribe((result) => {
          if (result) {
            this.instanceService.deallocateInstance(projectId, instance.id).subscribe({
              next: () => {
                this.instancesResource.reload();
                this.projectStateService.currentInstanceId.set(null);
              },
            });
          }
        });
    }
  }

  onAllocateInstance() {
    const projectId = this.projectStateService.currentProjectId();
    if (projectId) {
      this.instanceService.allocateInstance(projectId).subscribe({
        next: (instance) => {
          this.instancesResource.reload();
          this.projectStateService.currentInstanceId.set(instance.id);
          this.instanceAllocated.emit(instance);
        },
      });
    }
  }

  reload() {
    this.instancesResource.reload();
  }
}
