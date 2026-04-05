import { Component, input, output } from '@angular/core';
import { Observable } from 'rxjs';
import { MatTreeModule } from '@angular/material/tree';
import { ProjectFileNode } from '../../types/project';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TreeActionsMenuComponent } from '../selectable-tree/tree-actions-menu/tree-actions-menu.component';
import { ErrorResponse } from '../../types/api';

export interface VirtualStateNode {
  isVirtual: true;
  parentId: string | null;
  state: 'loading' | 'error' | 'load-more';
  error?: ErrorResponse;
}

export type FileTreeNode = ProjectFileNode | VirtualStateNode;

@Component({
  selector: 'app-lazy-file-tree',
  imports: [MatTreeModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, TreeActionsMenuComponent],
  templateUrl: './lazy-file-tree.component.html',
  styleUrls: ['./lazy-file-tree.component.scss'],
})
export class LazyFileTreeComponent {
  data = input.required<FileTreeNode[]>();
  enableDelete = input<boolean>(false);

  delete = output<ProjectFileNode>();
  clickChange = output<ProjectFileNode>();
  expand = output<ProjectFileNode>();
  retry = output<string | null>();
  loadMore = output<string | null>();

  childrenAccessor = input.required<(node: FileTreeNode) => FileTreeNode[] | Observable<FileTreeNode[]>>();

  isVirtualNode = (_: number, node: FileTreeNode): node is VirtualStateNode => 'isVirtual' in node;

  hasChild = (_: number, node: FileTreeNode): boolean => {
    if ('isVirtual' in node) return false;
    return node.type === 'folder';
  };

  trackByFn = (_: number, node: FileTreeNode) => ('isVirtual' in node ? node.state + node.parentId : node.id);
}
