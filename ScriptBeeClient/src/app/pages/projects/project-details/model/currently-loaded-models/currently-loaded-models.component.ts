import { Component, computed, input } from '@angular/core';
import { TreeNode } from '../../../../../types/tree-node';
import { SelectableTreeComponent } from '../../../../../components/tree/selectable-tree/selectable-tree.component';
import { Project, ProjectFile } from '../../../../../types/project';

@Component({
  selector: 'app-currently-loaded-models',
  templateUrl: './currently-loaded-models.component.html',
  styleUrls: ['./currently-loaded-models.component.scss'],
  imports: [SelectableTreeComponent],
})
export class CurrentlyLoadedModelsComponent {
  project = input.required<Project>();
  instanceId = input.required<string>();

  loadedFiles = computed<TreeNode<ProjectFile | string>[]>(() => {
    const project = this.project();

    const nodes: TreeNode<ProjectFile | string>[] = [];
    for (const loaderId of Object.keys(project.savedFiles)) {
      nodes.push({
        data: loaderId,
        children: project.savedFiles[loaderId].map((file) => ({ data: file })),
      });
    }
    return nodes;
  });

  displayNameAccessor = (node: TreeNode<ProjectFile | string>) => {
    if (typeof node.data === 'string') {
      return node.data;
    }
    return node.data.name;
  };
}
