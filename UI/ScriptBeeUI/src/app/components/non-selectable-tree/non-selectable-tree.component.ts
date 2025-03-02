import { Component, input } from '@angular/core';
import { MatTreeModule } from '@angular/material/tree';
import { TreeNode, TreeNodeWithParent } from '../../types/tree-node';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-non-selectable-tree',
  templateUrl: './non-selectable-tree.component.html',
  imports: [MatTreeModule, MatCheckboxModule, MatButtonModule, MatIcon],
  styleUrls: ['./non-selectable-tree.component.scss'],
})
export class NonSelectableTreeComponent {
  data = input.required<TreeNode[]>();

  childrenAccessor = (node: TreeNodeWithParent) => node.children ?? [];

  hasChild = (_: number, node: TreeNodeWithParent) => !!node.children && node.children.length > 0;
}
