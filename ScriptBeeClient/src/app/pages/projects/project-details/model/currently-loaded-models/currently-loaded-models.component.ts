﻿import { Component, computed, input } from '@angular/core';
import { TreeNode } from '../../../../../types/tree-node';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { InstanceInfo } from '../../../../../types/instance';

@Component({
  selector: 'app-currently-loaded-models',
  templateUrl: './currently-loaded-models.component.html',
  styleUrls: ['./currently-loaded-models.component.scss'],
  imports: [SelectableTreeComponent],
})
export class CurrentlyLoadedModelsComponent {
  instanceInfo = input.required<InstanceInfo>();

  loadedFiles = computed<TreeNode[]>(() => {
    // TODO FIXIT(#70): populate from api
    return convertToTreeNodes({});
  });
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
