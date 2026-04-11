import { Component, computed, effect, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ProjectStateService } from '../../services/projects/project-state.service';
import { InstanceInfo, InstanceStatus } from '../../types/instance';
import { ConfirmationDialogComponent } from '../dialogs/confirmation-dialog/confirmation-dialog.component';
import { InstanceAllocationService } from '../../services/instances/instance-allocation.service';

@Component({
  selector: 'app-instance-manager',
  imports: [MatButtonModule, MatIconModule, MatMenuModule, MatTooltipModule, MatDividerModule, MatDialogModule, DatePipe],
  templateUrl: './instance-manager.component.html',
  styleUrls: ['./instance-manager.component.scss'],
})
export class InstanceManagerComponent {
  private instanceAllocationService = inject(InstanceAllocationService);
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

  instances = computed(() => this.instanceAllocationService.instancesResource.value() ?? []);

  currentInstanceId = computed(() => this.projectStateService.currentInstanceId());

  currentInstance = computed(() => {
    const id = this.currentInstanceId();
    return this.instances().find((i) => i.id === id);
  });

  getStatusIcon(status: InstanceStatus): string {
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

  getStatusClass(status: InstanceStatus): string {
    return status.toLowerCase();
  }

  onSelectInstance(instance: InstanceInfo) {
    this.instanceAllocationService.setCurrentInstance(instance);
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
            this.instanceAllocationService.deallocateInstance(projectId, instance.id).subscribe();
          }
        });
    }
  }

  onAllocateInstance() {
    const projectId = this.projectStateService.currentProjectId();
    if (projectId) {
      this.instanceAllocationService.allocateInstance(projectId).subscribe();
    }
  }

  reload() {
    this.instanceAllocationService.reload();
  }
}
