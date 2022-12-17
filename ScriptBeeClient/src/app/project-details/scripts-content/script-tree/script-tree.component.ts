import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  selectScriptTree,
  selectScriptTreeLoading
} from '../../../state/script-tree/script-tree.selectors';
import { ScriptTreeNode } from '../../../state/script-tree/script-tree.state';
import { scriptTreeLeafClick } from '../../../state/script-tree/script-tree.actions';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss']
})
export class ScriptTreeComponent implements OnInit {
  loading = false;
  loadingError = '';
  fileStructureTree: ScriptTreeNode[] = [];

  // todo take into consideration the expanded state of the tree

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.select(selectScriptTreeLoading).subscribe({
      next: ({ loading, error }) => {
        this.loading = loading ?? false;
        this.loadingError = error ?? '';
      }
    });

    this.store.select(selectScriptTree).subscribe({
      next: (tree) => {
        this.fileStructureTree = tree;
      }
    });
  }

  onLeafClick($event) {
    this.store.dispatch(scriptTreeLeafClick({ node: $event }));
  }
}
