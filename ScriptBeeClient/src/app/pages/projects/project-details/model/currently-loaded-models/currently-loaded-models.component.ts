import { Component, computed, input } from '@angular/core';
import { TreeNode } from '../../../../../types/tree-node';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { Project } from '../../../../../types/project';

@Component({
  selector: 'app-currently-loaded-models',
  templateUrl: './currently-loaded-models.component.html',
  styleUrls: ['./currently-loaded-models.component.scss'],
  imports: [SelectableTreeComponent],
})
export class CurrentlyLoadedModelsComponent {
  project = input.required<Project>();
  instanceId = input.required<string>();

  loadedFiles = computed<TreeNode[]>(() => {
    const project = this.project();

    const nodes: TreeNode[] = [];
    for (const loaderId of Object.keys(project.savedFiles)) {
      nodes.push({
        name: loaderId,
        children: project.savedFiles[loaderId].map((file) => ({ name: file.name })),
      });
    }
    return nodes;
  });
}
