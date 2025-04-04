import { Component, computed, input } from '@angular/core';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { ProjectContextService } from '../../../../../services/projects/project-context.service';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { apiHandler } from '../../../../../utils/apiHandler';
import { MatButton } from '@angular/material/button';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { TreeNode } from '../../../../../types/tree-node';
import { ProjectContext } from '../../../../../types/returned-context-slice';

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

  projectContextResource = createRxResourceHandler({
    request: () => ({ projectId: this.projectId(), instanceId: this.instanceId() }),
    loader: (params) => this.projectContextService.getProjectContext(params.request.projectId, params.request.instanceId),
  });

  clearContextHandler = apiHandler(
    (params: { projectId: string; instanceId: string }) => this.projectContextService.clearContext(params.projectId, params.instanceId),
    () => this.projectContextResource.reload()
  );

  reloadContextHandler = apiHandler(
    (params: { projectId: string; instanceId: string }) => this.projectContextService.reloadContext(params.projectId, params.instanceId),
    () => this.projectContextResource.reload()
  );

  constructor(private projectContextService: ProjectContextService) {}

  onReloadModelsClick() {
    this.reloadContextHandler.execute({ projectId: this.projectId(), instanceId: this.instanceId() });
  }

  onClearContextButtonClick() {
    this.clearContextHandler.execute({ projectId: this.projectId(), instanceId: this.instanceId() });
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
