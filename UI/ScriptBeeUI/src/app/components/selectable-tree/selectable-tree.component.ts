import { Component, input, output } from '@angular/core';
import { MatTreeModule } from '@angular/material/tree';
import { TreeNode, TreeNodeWithParent } from '../../types/tree-node';
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
export class SelectableTreeComponent {
  data = input.required<TreeNode[]>();

  folderIcon = input<string | undefined>(undefined);
  fileIcon = input<string | undefined>(undefined);
  enableDelete = input<boolean>(false);

  onDelete = output<TreeNode>();

  childrenAccessor = (node: TreeNodeWithParent) => node.children ?? [];

  hasChild = (_: number, node: TreeNodeWithParent) => !!node.children && node.children.length > 0;
}
