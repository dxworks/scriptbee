import { Component, input, output } from '@angular/core';
import { MatTreeModule } from '@angular/material/tree';
import { TreeAction, TreeNode, TreeNodeWithParent } from '../../../types/tree-node';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { TreeActionsMenuComponent } from './tree-actions-menu/tree-actions-menu.component';

@Component({
  selector: 'app-selectable-tree',
  templateUrl: './selectable-tree.component.html',
  imports: [MatTreeModule, MatCheckboxModule, MatButtonModule, MatIconModule, MatMenuModule, TreeActionsMenuComponent],
  styleUrls: ['./selectable-tree.component.scss'],
})
export class SelectableTreeComponent<T> {
  data = input.required<TreeNode<T>[]>();

  folderIcon = input<string | undefined>(undefined);
  fileIcon = input<string | undefined>(undefined);
  actions = input<TreeAction<T>[]>([]);

  clickChange = output<TreeNode<T>>();

  childrenAccessor = (node: TreeNodeWithParent<T>) => node.children ?? [];
  displayNameAccessor = input.required<(node: TreeNode<T>) => string>();

  hasChild = (_: number, node: TreeNodeWithParent<T>) => !!node.children && node.children.length > 0;
}
