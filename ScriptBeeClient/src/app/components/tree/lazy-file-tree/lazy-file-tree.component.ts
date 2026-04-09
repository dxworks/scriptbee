import { Component, input, output } from '@angular/core';
import { Observable } from 'rxjs';
import { MatTreeModule } from '@angular/material/tree';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TreeActionsMenuComponent } from '../selectable-tree/tree-actions-menu/tree-actions-menu.component';
import { ErrorResponse } from '../../../types/api';
import { TreeAction, TreeNode } from '../../../types/tree-node';

export interface VirtualStateNode {
  isVirtual: true;
  parentId: string | null;
  state: 'loading' | 'error' | 'load-more';
  error?: ErrorResponse;
}

export type FileTreeNode<T> = TreeNode<T> | VirtualStateNode;

@Component({
  selector: 'app-lazy-file-tree',
  imports: [MatTreeModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, TreeActionsMenuComponent],
  templateUrl: './lazy-file-tree.component.html',
  styleUrls: ['./lazy-file-tree.component.scss'],
})
export class LazyFileTreeComponent<T> {
  data = input.required<FileTreeNode<T>[]>();
  actions = input<TreeAction<T>[]>([]);

  clickChange = output<TreeNode<T>>();
  expand = output<TreeNode<T>>();
  retry = output<string | null>();
  loadMore = output<string | null>();

  childrenAccessor = input.required<(node: FileTreeNode<T>) => FileTreeNode<T>[] | Observable<FileTreeNode<T>[]>>();

  isVirtualNode = (_: number, node: FileTreeNode<T>): node is VirtualStateNode => 'isVirtual' in node;

  hasChildAccessor = input.required<(node: TreeNode<T>) => boolean>();
  idAccessor = input.required<(node: TreeNode<T>) => string>();

  hasChild = (_: number, node: FileTreeNode<T>): boolean => {
    if ('isVirtual' in node) return false;
    return this.hasChildAccessor()(node);
  };

  trackByFn = (_: number, node: FileTreeNode<T>) => ('isVirtual' in node ? node.state + node.parentId : this.idAccessor()(node));
}
