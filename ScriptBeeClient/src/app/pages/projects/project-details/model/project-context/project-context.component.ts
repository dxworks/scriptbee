import { Component, computed, input, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProjectContextService } from '../../../../../services/projects/project-context.service';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { MatButton } from '@angular/material/button';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { TreeNode } from '../../../../../types/tree-node';
import { ProjectContext } from '../../../../../types/returned-context-slice';
import { finalize } from 'rxjs';
import { convertError } from '../../../../../utils/api';

@Component({
  selector: 'app-project-context',
  templateUrl: './project-context.component.html',
  styleUrls: ['./project-context.component.scss'],
  imports: [CenteredSpinnerComponent, ErrorStateComponent, MatButton, SelectableTreeComponent],
})
export class ProjectContextComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();

  context = computed<TreeNode[]>(() => {
    return convertToTreeNodes(this.projectContextResource.value() ?? []);
  });

  projectContextResource = rxResource({
    params: () => ({ projectId: this.projectId(), instanceId: this.instanceId() }),
    stream: ({ params }) => this.projectContextService.getProjectContext(params.projectId, params.instanceId),
  });
  projectContextResourceError = computed(() => convertError(this.projectContextResource.error()));

  isClearContextLoading = signal(false);
  isReloadContextLoading = signal(false);

  constructor(private projectContextService: ProjectContextService) {}

  onReloadModelsClick() {
    this.isReloadContextLoading.set(true);
    this.projectContextService
      .reloadContext(this.projectId(), this.instanceId())
      .pipe(finalize(() => this.isReloadContextLoading.set(false)))
      .subscribe({ next: () => this.projectContextResource.reload() });
  }

  onClearContextButtonClick() {
    this.isClearContextLoading.set(true);
    this.projectContextService
      .clearContext(this.projectId(), this.instanceId())
      .pipe(finalize(() => this.isClearContextLoading.set(false)))
      .subscribe({ next: () => this.projectContextResource.reload() });
  }
}

function convertToTreeNodes(context: ProjectContext): TreeNode[] {
  const nodes: TreeNode[] = [];

  for (const contextSlice of context) {
    nodes.push({
      name: contextSlice.model,
      children: contextSlice.pluginIds.map((pluginId) => ({ name: pluginId })),
    });
  }

  return nodes;
}
