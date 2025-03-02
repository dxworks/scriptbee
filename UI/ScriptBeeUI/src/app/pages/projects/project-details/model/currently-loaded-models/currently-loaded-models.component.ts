import { Component, computed, input } from '@angular/core';
import { InstanceService } from '../../../../../services/instances/instance.service';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { TreeNode } from '../../../../../types/tree-node';
import { NonSelectableTreeComponent } from '../../../../../components/non-selectable-tree/non-selectable-tree.component';

@Component({
  selector: 'app-currently-loaded-models',
  templateUrl: './currently-loaded-models.component.html',
  styleUrls: ['./currently-loaded-models.component.scss'],
  imports: [CenteredSpinnerComponent, ErrorStateComponent, NonSelectableTreeComponent],
})
export class CurrentlyLoadedModelsComponent {
  projectId = input.required<string>();

  getCurrentInstanceInfoResource = createRxResourceHandler({
    request: () => this.projectId(),
    loader: (params) => this.instanceService.getCurrentInstance(params.request),
  });

  loadedFiles = computed<TreeNode[]>(() => {
    const instanceInfo = this.getCurrentInstanceInfoResource.value();

    if (!instanceInfo) {
      return [];
    }

    return convertToTreeNodes(instanceInfo.loadedModels);
  });

  constructor(private instanceService: InstanceService) {}
}

function convertToTreeNodes(loadedModels: Record<string, string[]>): TreeNode[] {
  const nodes: TreeNode[] = [];

  for (const loaderId of Object.keys(loadedModels)) {
    nodes.push({
      name: loaderId,
      children: loadedModels[loaderId].map((model) => ({ name: model })),
    });
  }

  return nodes;
}
