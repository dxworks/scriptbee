import { Component, input, output } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTreeModule } from '@angular/material/tree';
import { TreeAction, TreeNode, TreeNodeWithParent } from '../../../types/tree-node';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TreeActionsMenuComponent } from '../selectable-tree/tree-actions-menu/tree-actions-menu.component';

@Component({
  selector: 'app-checkable-tree',
  templateUrl: './checkable-tree.component.html',
  imports: [MatTreeModule, MatCheckboxModule, MatButtonModule, MatIcon, TreeActionsMenuComponent],
  styleUrls: ['./checkable-tree.component.scss'],
})
export class CheckableTreeComponent<T> {
  data = input.required<TreeNodeWithParent<T>[], TreeNode<T>[]>({ transform: transformToUITreeNode });
  actions = input<TreeAction<T>[]>([]);
  updateCheckedFiles = output<TreeNode<T>[]>();

  selection = new SelectionModel<TreeNodeWithParent<T>>(true);

  childrenAccessor = (node: TreeNodeWithParent<T>) => node.children ?? [];

  hasChild = (_: number, node: TreeNodeWithParent<T>) => !!node.children && node.children.length > 0;

  isIndeterminate(node: TreeNodeWithParent<T>): boolean {
    if (!node.children || node.children.length === 0) {
      return false;
    }
    const selectedChildren = node.children.filter((child) => this.selection.isSelected(child));
    const indeterminateChildren = node.children.some((child) => this.isIndeterminate(child));
    return indeterminateChildren || (selectedChildren.length > 0 && selectedChildren.length < node.children.length);
  }

  toggleSelection(node: TreeNodeWithParent<T>): void {
    this.selection.toggle(node);
    const isSelected = this.selection.isSelected(node);
    if (node.children && node.children.length > 0) {
      this.toggleChildrenSelection(node, isSelected);
    }

    this.updateParentSelection(node.parent);

    this.updateCheckedFiles.emit(this.selection.selected);
  }

  private toggleChildrenSelection(node: TreeNodeWithParent<T>, isSelected: boolean): void {
    node.children?.forEach((child) => {
      if (isSelected) {
        this.selection.select(child);
      } else {
        this.selection.deselect(child);
      }
      if (child.children && child.children.length > 0) {
        this.toggleChildrenSelection(child, isSelected);
      }
    });
  }

  private updateParentSelection(node?: TreeNodeWithParent<T>): void {
    if (!node) return;
    const allSelected = node.children?.every((child) => this.selection.isSelected(child)) ?? false;

    if (allSelected) {
      this.selection.select(node);
    } else {
      this.selection.deselect(node);
    }

    this.updateParentSelection(node.parent);
  }
}

function transformToUITreeNode<T>(nodes: TreeNode<T>[], parent?: TreeNodeWithParent<T>): TreeNodeWithParent<T>[] {
  return nodes.map((node) => {
    const newNode: TreeNodeWithParent<T> = {
      name: node.name,
      data: node.data,
      children: undefined,
      parent,
    };

    if (node.children) {
      newNode.children = transformToUITreeNode(node.children, newNode);
    }

    return newNode;
  });
}
