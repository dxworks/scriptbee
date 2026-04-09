import { Component, input } from '@angular/core';
import { MatTreeModule } from '@angular/material/tree';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { ClickStopPropagation } from '../../../../directives/click-stop-propagation';
import { TreeAction, TreeNode } from '../../../../types/tree-node';

@Component({
  selector: 'app-tree-actions-menu',
  templateUrl: './tree-actions-menu.component.html',
  imports: [MatTreeModule, MatCheckboxModule, MatButtonModule, MatIconModule, MatMenuModule, ClickStopPropagation],
  styleUrls: ['./tree-actions-menu.component.scss'],
})
export class TreeActionsMenuComponent<T> {
  actions = input<TreeAction<T>[]>([]);
  node = input.required<TreeNode<T>>();
}
