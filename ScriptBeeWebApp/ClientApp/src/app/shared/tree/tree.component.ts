import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NestedTreeControl} from '@angular/cdk/tree';
import {MatTreeNestedDataSource} from '@angular/material/tree';
import {TreeNode} from "../tree-node";

@Component({
  selector: 'appTree',
  templateUrl: './tree.component.html',
  styleUrls: ['./tree.component.scss']
})
export class TreeComponent {

  @Input() set treeData(value: TreeNode[]) {
    this.dataSource.data = value;
  }

  @Output() leafClick = new EventEmitter<TreeNode>();

  treeControl = new NestedTreeControl<TreeNode>(node => node.children);
  dataSource = new MatTreeNestedDataSource<TreeNode>();

  constructor() {
    this.dataSource.data = [];
  }

  hasChild = (_: number, node: TreeNode) => !!node.children && node.children.length >= 0;

  onLeafNodeClick(node: TreeNode) {
    this.leafClick.emit(node);
  }
}
