import { Component, computed, input, output, signal, effect, Injector, linkedSignal, inject } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { MatIconButton } from '@angular/material/button';
import { MatTooltip } from '@angular/material/tooltip';
import { MatIcon } from '@angular/material/icon';
import { FileTreeNode, LazyFileTreeComponent, VirtualStateNode } from '../../../../../components/tree/lazy-file-tree/lazy-file-tree.component';
import { ProjectFileNode } from '../../../../../types/project';
import { MatDialog } from '@angular/material/dialog';
import { CreateScriptDialogComponent } from './create-script-dialog/create-script-dialog.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ProjectStructureService } from '../../../../../services/projects/project-structure.service';
import { finalize } from 'rxjs';
import { ErrorResponse } from '../../../../../types/api';
import { convertError } from '../../../../../utils/api';
import { TreeAction, TreeNode } from '../../../../../types/tree-node';

interface FolderState {
  data: TreeNode<ProjectFileNode>[];
  status: 'idle' | 'loading' | 'error' | 'loaded';
  error?: ErrorResponse;
  totalCount: number;
}

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  imports: [MatIconButton, MatTooltip, MatIcon, LazyFileTreeComponent, ErrorStateComponent, LoadingProgressBarComponent],
})
export class ScriptTreeComponent {
  projectId = input.required<string>();

  fileSelected = output<ProjectFileNode>();

  isDeleteLoading = signal(false);

  actions: TreeAction<ProjectFileNode>[] = [
    {
      label: 'Delete',
      icon: 'delete',
      callback: (node) => this.deleteNode(node.data),
    },
  ];

  private injector = inject(Injector);

  private folderStates = linkedSignal<string, Map<string | null, FolderState>>({
    source: this.projectId,
    computation: () => new Map(),
  });

  rootData = computed(() => {
    return this.getChildren(null);
  });

  rootError = computed(() => {
    const rootState = this.folderStates().get(null);
    if (rootState?.status === 'error') {
      return rootState.error;
    }
    return null;
  });

  private projectStructureService = inject(ProjectStructureService);
  private dialog = inject(MatDialog);

  constructor() {
    effect(() => {
      const pid = this.projectId();
      if (pid && !this.folderStates().has(null)) {
        this.loadFolder(null);
      }
    });
  }

  onCreateNewScriptButtonClick() {
    this.dialog.open(CreateScriptDialogComponent, {
      disableClose: true,
      data: { projectId: this.projectId() },
    });
  }

  private deleteNode(node: ProjectFileNode) {
    this.isDeleteLoading.set(true);
    this.projectStructureService
      .deleteProjectStructureNode(this.projectId(), node.id)
      .pipe(finalize(() => this.isDeleteLoading.set(false)))
      .subscribe({
        next: () => {
          this.folderStates.set(new Map());
          this.loadFolder(null);
        },
      });
  }

  onNodeClick(node: TreeNode<ProjectFileNode>) {
    if (node.data.type === 'file') {
      this.fileSelected.emit(node.data);
    }
  }

  onExpand(node: TreeNode<ProjectFileNode>) {
    if (node.data.type === 'folder' && !this.folderStates().has(node.data.id)) {
      this.loadFolder(node.data.id);
    }
  }

  onRetry(parentId: string | null) {
    this.loadFolder(parentId);
  }

  onLoadMore(parentId: string | null) {
    this.loadFolder(parentId);
  }

  hasChildAccessor = (node: TreeNode<ProjectFileNode>) => node.data.hasChildren;
  idAccessor = (node: TreeNode<ProjectFileNode>) => node.data.id;

  childrenAccessor = (node: FileTreeNode<ProjectFileNode>) => {
    if ('isVirtual' in node) return [];
    return toObservable(
      computed(() => this.getChildren(node.data.id)),
      { injector: this.injector }
    );
  };

  private getChildren(parentId: string | null): FileTreeNode<ProjectFileNode>[] {
    const state = this.folderStates().get(parentId);
    if (!state) {
      return [];
    }

    const nodes: FileTreeNode<ProjectFileNode>[] = [...state.data];

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

    this.projectStructureService.getProjectFiles(this.projectId(), parentId || undefined, currentOffset, 50).subscribe({
      next: (response) => {
        this.folderStates.update((map) => {
          const newMap = new Map(map);
          const previousData = newMap.get(parentId)?.data || [];
          const mappedResponse = response.data.map((d) => ({ name: d.name, data: d }));
          const combinedData = [...previousData, ...mappedResponse];
          const isFullyLoaded = combinedData.length >= response.totalCount || response.data.length === 0;

          newMap.set(parentId, {
            data: combinedData,
            totalCount: response.totalCount,
            status: isFullyLoaded ? 'loaded' : 'idle',
          });
          return newMap;
        });
      },
      error: (err) => {
        this.folderStates.update((map) => {
          const newMap = new Map(map);
          newMap.set(parentId, { ...currentState, status: 'error', error: convertError(err) });
          return newMap;
        });
      },
    });
  }
}
