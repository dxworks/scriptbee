import {Component, Input} from '@angular/core';
import {ScriptsStore} from '../../../../stores/scripts-store.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ScriptFileStructureNode} from '../../../../services/script-types';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  providers: [ScriptsStore],
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

  private _projectId: string;

  scriptsForProject$ = this.store.scriptsForProject;
  scriptsForProjectError$ = this.store.scriptsForProjectError;
  scriptsForProjectLoading$ = this.store.scriptsForProjectLoading;

  // todo take into consideration the expanded state of the tree

  constructor(private store: ScriptsStore, private route: ActivatedRoute, private router: Router) {}

  onLeafClick(node: ScriptFileStructureNode) {
    this.router.navigate([node.path], { relativeTo: this.route }).then();
  }

  onDeleteLeafClick(node: ScriptFileStructureNode) {
    this.store.deleteScript({ scriptId: node.path, projectId: this.projectId });
  }
}
