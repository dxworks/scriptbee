import { Component, Input } from '@angular/core';
import { ScriptsStore } from '../../../../stores/scripts-store.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ScriptFileStructureNode } from '../../../../services/script-types';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
})
export class ScriptTreeComponent {
  get projectId(): string {
    return this._projectId;
  }

  @Input()
  set projectId(value: string) {
    this._projectId = value;
    if (this._projectId) {
      this.store.loadScriptsForProject(this._projectId);
    }
  }

  treeControl = new NestedTreeControl<ScriptFileStructureNode>((node) => node.children);
  dataSource = new MatTreeNestedDataSource<ScriptFileStructureNode>();

  private _projectId: string;

  scriptsForProjectError$ = this.store.scriptsForProjectError;
  scriptsForProjectLoading$ = this.store.scriptsForProjectLoading;

  // todo take into consideration the expanded state of the tree

  constructor(private store: ScriptsStore, private route: ActivatedRoute, private router: Router) {
    this.dataSource.data = [];

    this.store.scriptsForProject.subscribe((scripts) => {
      this.dataSource.data = scripts;
    });
  }

  hasChild = (_: number, node: ScriptFileStructureNode) => !!node.children && node.children.length >= 0;

  onLeafClick(node: ScriptFileStructureNode) {
    this.router.navigate([node.path], { relativeTo: this.route }).then();
  }

  onDeleteLeafClick(node: ScriptFileStructureNode) {
    this.store.deleteScript({ scriptId: node.path, projectId: this.projectId });
  }
}
