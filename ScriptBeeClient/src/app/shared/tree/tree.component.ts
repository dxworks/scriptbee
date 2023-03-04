import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NestedTreeControl} from '@angular/cdk/tree';
import {MatTreeNestedDataSource} from '@angular/material/tree';
import {ScriptFileStructureNode} from '../../project-details/services/script-types';

@Component({
  selector: 'app-tree',
  templateUrl: './tree.component.html',
  styleUrls: ['./tree.component.scss'],
})
export class TreeComponent {
  @Input() set treeData(value: ScriptFileStructureNode[]) {
    this.dataSource.data = value;
  }

  @Output() leafClick = new EventEmitter<ScriptFileStructureNode>();
  @Output() deleteLeafClick = new EventEmitter<ScriptFileStructureNode>();

  treeControl = new NestedTreeControl<ScriptFileStructureNode>((node) => node.children);
  dataSource = new MatTreeNestedDataSource<ScriptFileStructureNode>();

  constructor() {
    this.dataSource.data = [];
  }

  hasChild = (_: number, node: ScriptFileStructureNode) => !!node.children && node.children.length >= 0;

  onLeafNodeClick(node: ScriptFileStructureNode) {
    this.leafClick.emit(node);
  }

  onDeleteLeafClick(node: ScriptFileStructureNode) {
    this.deleteLeafClick.emit(node);
  }
}
