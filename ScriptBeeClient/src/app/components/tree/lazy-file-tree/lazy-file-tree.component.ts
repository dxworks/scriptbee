import { Component, computed, inject, Injector, input, output, signal, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MatTreeModule } from '@angular/material/tree';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TreeActionsMenuComponent } from '../selectable-tree/tree-actions-menu/tree-actions-menu.component';
import { ErrorResponse } from '../../../types/api';
import { TreeAction, TreeNode } from '../../../types/tree-node';
import { toObservable } from '@angular/core/rxjs-interop';
import { NgClass } from '@angular/common';

export interface VirtualStateNode {
  isVirtual: true;
  parentId: string | null;
  state: 'loading' | 'error' | 'load-more';
  error?: ErrorResponse;
}

export type FileTreeNode<T> = TreeNode<T> | VirtualStateNode;

interface FolderState<T> {
  data: TreeNode<T>[];
  status: 'idle' | 'loading' | 'error' | 'loaded';
  error?: ErrorResponse;
  totalCount: number;
}

export interface FetchResult<T> {
  data: TreeNode<T>[];
  totalCount: number;
  error?: ErrorResponse;
}

@Component({
  selector: 'app-lazy-file-tree',
  imports: [MatTreeModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, TreeActionsMenuComponent, NgClass],
  templateUrl: './lazy-file-tree.component.html',
  styleUrls: ['./lazy-file-tree.component.scss'],
})
export class LazyFileTreeComponent<T> implements OnInit {
  fetchData = input.required<(parentId: string | null, offset: number, limit: number) => Observable<FetchResult<T>>>();
  limit = input<number>(50);

  actions = input<TreeAction<T>[]>([]);

  fileActions = computed(() => {
    return this.actions().filter((action) => action.type === 'file' || action.type === 'all');
  });

  folderActions = computed(() => {
    return this.actions().filter((action) => action.type === 'folder' || action.type === 'all');
  });

  clickChange = output<TreeNode<T>>();

  hasChildAccessor = input.required<(node: TreeNode<T>) => boolean>();
  idAccessor = input.required<(node: TreeNode<T>) => string>();
  displayNameAccessor = input.required<(node: TreeNode<T>) => string>();
  selectedAccessor = input.required<(node: TreeNode<T>) => boolean>();

  private folderStates = signal<Map<string | null, FolderState<T>>>(new Map());
  private childrenObservables = new Map<string, Observable<FileTreeNode<T>[]>>();

  private injector = inject(Injector);

  ngOnInit() {
    this.loadFolder(null);
  }

  rootData = computed(
    () => {
      const state = this.folderStates().get(null);
      if (!state) {
        return [] as FileTreeNode<T>[];
      }
      return state.data;
    },
    { equal: (a, b) => a === b || (a.length === b.length && a.every((v, i) => v === b[i])) }
  );

  childrenAccessor = (node: FileTreeNode<T>): Observable<FileTreeNode<T>[]> | FileTreeNode<T>[] => {
    if ('isVirtual' in node) {
      return [];
    }
    const id = this.idAccessor()(node);
    if (!this.childrenObservables.has(id)) {
      this.childrenObservables.set(
        id,
        toObservable(
          computed(() => this.getChildren(id)),
          { injector: this.injector }
        )
      );
    }
    return this.childrenObservables.get(id)!;
  };

  isVirtualNode = (_: number, node: FileTreeNode<T>): node is VirtualStateNode => 'isVirtual' in node;

  hasChild = (_: number, node: FileTreeNode<T>): boolean => {
    if ('isVirtual' in node) {
      return false;
    }
    return this.hasChildAccessor()(node);
  };

  trackByFn = (_: number, node: FileTreeNode<T>) => {
    if ('isVirtual' in node) {
      return node.state + node.parentId;
    }
    return this.idAccessor()(node);
  };

  onExpand(node: TreeNode<T>) {
    const parentId = this.idAccessor()(node);
    if (!this.folderStates().has(parentId)) {
      this.loadFolder(parentId);
    }
  }

  onRetry(parentId: string | null) {
    this.loadFolder(parentId);
  }

  onLoadMore(parentId: string | null) {
    this.loadFolder(parentId);
  }

  public updateNode(id: string, updater: (old: TreeNode<T>) => TreeNode<T>) {
    this.folderStates.update((map) => {
      const newMap = new Map(map);
      for (const [parentId, state] of newMap) {
        const idx = state.data.findIndex((n) => this.idAccessor()(n) === id);
        if (idx !== -1) {
          const updatedData = [...state.data];
          updatedData[idx] = updater(state.data[idx]);
          newMap.set(parentId, { ...state, data: updatedData });
          break;
        }
      }
      return newMap;
    });
  }

  public removeNode(id: string) {
    this.folderStates.update((map) => {
      const newMap = new Map(map);
      for (const [parentId, state] of newMap) {
        const idx = state.data.findIndex((n) => this.idAccessor()(n) === id);
        if (idx !== -1) {
          const updatedData = [...state.data];
          updatedData.splice(idx, 1);
          newMap.set(parentId, { ...state, data: updatedData, totalCount: state.totalCount - 1 });
          break;
        }
      }
      return newMap;
    });
  }

  public reloadFolder(parentId: string | null = null) {
    this.folderStates.update((map) => {
      const newMap = new Map(map);
      newMap.delete(parentId);
      return newMap;
    });
    this.loadFolder(parentId);
  }

  public resetTree() {
    this.folderStates.set(new Map());
    this.childrenObservables.clear();
    this.loadFolder(null);
  }

  private getChildren(parentId: string | null): FileTreeNode<T>[] {
    const state = this.folderStates().get(parentId);
    if (!state) {
      return [];
    }

    const nodes: FileTreeNode<T>[] = [...state.data];

    if (state.status === 'loading') {
      nodes.push({ isVirtual: true, parentId, state: 'loading' } as VirtualStateNode);
    } else if (state.status === 'error') {
      nodes.push({ isVirtual: true, parentId, state: 'error', error: state.error } as VirtualStateNode);
    } else if (state.status === 'idle' && state.data.length > 0 && state.data.length < state.totalCount) {
      nodes.push({ isVirtual: true, parentId, state: 'load-more' } as VirtualStateNode);
    }

    return nodes;
  }

  private loadFolder(parentId: string | null) {
    const states = this.folderStates();
    const currentState = states.get(parentId) || {
      data: [],
      status: 'idle',
      totalCount: -1,
    };

    if (currentState.status === 'loading' || currentState.status === 'loaded') {
      return;
    }

    const currentOffset = currentState.data.length;

    this.folderStates.update((map) => {
      const newMap = new Map(map);
      newMap.set(parentId, { ...currentState, status: 'loading' });
      return newMap;
    });

    this.fetchData()(parentId, currentOffset, this.limit()).subscribe({
      next: (response) => {
        this.folderStates.update((map) => {
          const newMap = new Map(map);
          const previousData = newMap.get(parentId)?.data || [];
          const combinedData = [...previousData, ...response.data];
          const isFullyLoaded = combinedData.length >= response.totalCount || response.data.length === 0;

          newMap.set(parentId, {
            data: combinedData,
            totalCount: response.totalCount,
            status: isFullyLoaded ? 'loaded' : 'idle',
          });
          return newMap;
        });
      },
      error: (error) => {
        this.folderStates.update((map) => {
          const newMap = new Map(map);
          const mappedError = error?.error ? error.error : error;
          newMap.set(parentId, { ...currentState, status: 'error', error: mappedError });
          return newMap;
        });
      },
    });
  }
}
