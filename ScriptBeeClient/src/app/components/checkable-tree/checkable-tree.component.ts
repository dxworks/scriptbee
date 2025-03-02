import { Component, input, output } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTreeModule } from '@angular/material/tree';
import { TreeNode, TreeNodeWithParent } from '../../types/tree-node';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-checkable-tree',
  templateUrl: './checkable-tree.component.html',
  imports: [MatTreeModule, MatCheckboxModule, MatButtonModule, MatIcon],
  styleUrls: ['./checkable-tree.component.scss'],
})
export class CheckableTreeComponent {
  data = input.required<TreeNodeWithParent[], TreeNode[]>({ transform: transformToUITreeNode });
  updateCheckedFiles = output<TreeNode[]>();

  selection = new SelectionModel<TreeNodeWithParent>(true);

  childrenAccessor = (node: TreeNodeWithParent) => node.children ?? [];

  hasChild = (_: number, node: TreeNodeWithParent) => !!node.children && node.children.length > 0;

  isIndeterminate(node: TreeNodeWithParent): boolean {
    if (!node.children || node.children.length === 0) {
      return false;
    }
    const selectedChildren = node.children.filter((child) => this.selection.isSelected(child));
    const indeterminateChildren = node.children.some((child) => this.isIndeterminate(child));
    return indeterminateChildren || (selectedChildren.length > 0 && selectedChildren.length < node.children.length);
  }

  toggleSelection(node: TreeNodeWithParent): void {
    this.selection.toggle(node);
    const isSelected = this.selection.isSelected(node);
    if (node.children && node.children.length > 0) {
      this.toggleChildrenSelection(node, isSelected);
    }

    this.updateParentSelection(node.parent);

    this.updateCheckedFiles.emit(this.selection.selected);
  }

  private toggleChildrenSelection(node: TreeNodeWithParent, isSelected: boolean): void {
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

  private updateParentSelection(node?: TreeNodeWithParent): void {
    if (!node) return;
    const allSelected = node.children?.every((child) => this.selection.isSelected(child)) ?? false;
    const anySelected = node.children?.some((child) => this.selection.isSelected(child));
    const anyIndeterminate = node.children?.some((child) => this.isIndeterminate(child));

    if (allSelected) {
      this.selection.select(node);
    } else if (anySelected || anyIndeterminate) {
      this.selection.deselect(node);
    } else {
      this.selection.deselect(node);
    }

    this.updateParentSelection(node.parent);
  }
}

function transformToUITreeNode(nodes: TreeNode[], parent?: TreeNodeWithParent): TreeNodeWithParent[] {
  return nodes.map((node) => {
    const newNode: TreeNodeWithParent = {
      name: node.name,
      children: node.children ? transformToUITreeNode(node.children) : undefined,
      parent,
    };

    if (newNode.children) {
      newNode.children.forEach((child) => (child.parent = newNode));
    }

    return newNode;
  });
}
