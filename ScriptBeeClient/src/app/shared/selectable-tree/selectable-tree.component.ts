import {Component, EventEmitter, Input, Output} from '@angular/core';
import {SelectionModel} from '@angular/cdk/collections';
import {FlatTreeControl} from '@angular/cdk/tree';
import {MatTreeFlatDataSource, MatTreeFlattener} from '@angular/material/tree';
import {TreeNode} from "../tree-node";

@Component({
  selector: 'app-selectable-tree',
  templateUrl: './selectable-tree.component.html',
  styleUrls: ['./selectable-tree.component.scss']
})
export class SelectableTreeComponent {

  @Input() set treeData(value: TreeNode[]) {
    this.dataSource.data = value;
  }

  @Output() updateCheckedFiles = new EventEmitter<TreeNode[]>();

  flatNodeMap = new Map<FlatNode, TreeNode>();

  nestedNodeMap = new Map<TreeNode, FlatNode>();

  selectedParent: FlatNode | null = null;

  treeControl: FlatTreeControl<FlatNode>;

  treeFlattener: MatTreeFlattener<TreeNode, FlatNode>;

  dataSource: MatTreeFlatDataSource<TreeNode, FlatNode>;

  checklistSelection = new SelectionModel<FlatNode>(true);

  constructor() {
    this.treeFlattener = new MatTreeFlattener(
      this.transformer,
      this.getLevel,
      this.isExpandable,
      this.getChildren,
    );
    this.treeControl = new FlatTreeControl<FlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
  }

  getLevel = (node: FlatNode) => node.level;

  isExpandable = (node: FlatNode) => node.expandable;

  getChildren = (node: TreeNode): TreeNode[] => node.children;

  hasChild = (_: number, _nodeData: FlatNode) => _nodeData.expandable;

  hasNoContent = (_: number, _nodeData: FlatNode) => _nodeData.name === '';

  transformer = (node: TreeNode, level: number) => {
    const existingNode = this.nestedNodeMap.get(node);
    const flatNode: FlatNode =
      existingNode && existingNode.name === node.name ? existingNode : node as FlatNode;

    flatNode.level = level;
    flatNode.expandable = !!node.children?.length;
    this.flatNodeMap.set(flatNode, node);
    this.nestedNodeMap.set(node, flatNode);
    return flatNode;
  };

  descendantsAllSelected(node: FlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected =
      descendants.length > 0 &&
      descendants.every(child => {
        return this.checklistSelection.isSelected(child);
      });
    return descAllSelected;
  }

  descendantsPartiallySelected(node: FlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const result = descendants.some(child => this.checklistSelection.isSelected(child));
    return result && !this.descendantsAllSelected(node);
  }

  itemSelectionToggle(node: FlatNode): void {
    this.checklistSelection.toggle(node);
    const descendants = this.treeControl.getDescendants(node);
    this.checklistSelection.isSelected(node)
      ? this.checklistSelection.select(...descendants)
      : this.checklistSelection.deselect(...descendants);

    descendants.forEach(child => this.checklistSelection.isSelected(child));
    this.checkAllParentsSelection(node);

    this.updateCheckedFiles.emit(this.getSelectedNodes());
  }

  leafItemSelectionToggle(node: FlatNode): void {
    this.checklistSelection.toggle(node);
    this.checkAllParentsSelection(node);

    this.updateCheckedFiles.emit(this.getSelectedNodes());
  }

  checkAllParentsSelection(node: FlatNode): void {
    let parent: FlatNode | null = this.getParentNode(node);
    while (parent) {
      this.checkRootNodeSelection(parent);
      parent = this.getParentNode(parent);
    }
  }

  checkRootNodeSelection(node: FlatNode): void {
    const nodeSelected = this.checklistSelection.isSelected(node);
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected =
      descendants.length > 0 &&
      descendants.every(child => {
        return this.checklistSelection.isSelected(child);
      });
    if (nodeSelected && !descAllSelected) {
      this.checklistSelection.deselect(node);
    } else if (!nodeSelected && descAllSelected) {
      this.checklistSelection.select(node);
    }
  }

  getParentNode(node: FlatNode): FlatNode | null {
    const currentLevel = this.getLevel(node);

    if (currentLevel < 1) {
      return null;
    }

    const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;

    for (let i = startIndex; i >= 0; i--) {
      const currentNode = this.treeControl.dataNodes[i];

      if (this.getLevel(currentNode) < currentLevel) {
        return currentNode;
      }
    }
    return null;
  }

  getSelectedNodes() {
    const dictionary = new Map<string, TreeNode[]>();
    this.checklistSelection.selected.forEach(node => {
      const parent = this.getParentNode(node);

      if (parent) {
        const children = dictionary.get(parent.name);
        if (!children) {
          dictionary.set(parent.name, [node]);
        } else {
          children.push(node);
        }
      } else {
        if (!dictionary.has(node.name)) {
          dictionary.set(node.name, []);
        }
      }
    });

    return Array.from(dictionary.entries(), ([parentName, children]) => {
      return {name: parentName, children: children};
    });
  }
}

export interface FlatNode {
  name: string;
  level: number;
  expandable: boolean;
}
