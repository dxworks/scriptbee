import { Component, inject, input, output, signal, viewChild } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatTooltip } from '@angular/material/tooltip';
import { MatIcon } from '@angular/material/icon';
import { FetchResult, LazyFileTreeComponent } from '../../../../../components/tree/lazy-file-tree/lazy-file-tree.component';
import { ProjectFile, ProjectFileNode } from '../../../../../types/project';
import { MatDialog } from '@angular/material/dialog';
import { CreateScriptDialogComponent } from './create-script-dialog/create-script-dialog.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ProjectStructureService } from '../../../../../services/projects/project-structure.service';
import { finalize, map } from 'rxjs';
import { convertError } from '../../../../../utils/api';
import { TreeAction, TreeNode } from '../../../../../types/tree-node';
import { RenameFileDialog } from '../../../../../components/dialogs/rename-file-dialog/rename-file-dialog.component';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProjectContextService } from '../../../../../services/projects/project-context.service';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  imports: [MatIconButton, MatTooltip, MatIcon, LazyFileTreeComponent, LoadingProgressBarComponent],
})
export class ScriptTreeComponent {
  projectId = input.required<string>();

  fileSelected = output<ProjectFileNode>();

  isDeleteLoading = signal(false);
  isGenerateClassesLoading = signal(false);

  lazyTree = viewChild.required(LazyFileTreeComponent<ProjectFileNode>);

  actions: TreeAction<ProjectFileNode>[] = [
    {
      label: 'Delete',
      icon: 'delete',
      type: 'all',
      callback: (node) => this.deleteNode(node.data),
    },
    {
      label: 'Rename',
      icon: 'edit',
      type: 'file',
      callback: (node) => this.renameFile(node.data),
    },
  ];

  hasChildAccessor = (node: TreeNode<ProjectFileNode>) => node.data.hasChildren;
  idAccessor = (node: TreeNode<ProjectFileNode>) => node.data.id;
  displayNameAccessor = (node: TreeNode<ProjectFileNode>) => node.data.name;

  private projectStructureService = inject(ProjectStructureService);
  private dialog = inject(MatDialog);
  private snackbar = inject(MatSnackBar);
  private projectContextService = inject(ProjectContextService);
  projectStateService = inject(ProjectStateService);

  fetchData = (parentId: string | null, offset: number, limit: number) => {
    return this.projectStructureService.getProjectFiles(this.projectId(), parentId || undefined, offset, limit).pipe(
      map((response) => {
        const fetchResult: FetchResult<ProjectFileNode> = {
          data: response.data.map((d) => ({ data: d })),
          totalCount: response.totalCount,
        };
        return fetchResult;
      })
    );
  };

  onCreateNewScriptButtonClick() {
    const dialogRef = this.dialog.open(CreateScriptDialogComponent, {
      disableClose: true,
      data: { projectId: this.projectId() },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.onReloadTreeButtonClick();
      }
    });
  }

  onReloadTreeButtonClick() {
    this.lazyTree().resetTree();
  }

  onGenerateClassesClick() {
    const instanceId = this.projectStateService.currentInstanceId();
    if (!instanceId) {
      this.snackbar.open('Cannot generate classes without an active instance', 'Dismiss', { duration: 4000 });
      return;
    }

    this.isGenerateClassesLoading.set(true);
    this.projectContextService
      .generateClasses(this.projectId(), instanceId)
      .pipe(finalize(() => this.isGenerateClassesLoading.set(false)))
      .subscribe({
        next: () => {
          this.snackbar.open('Successfully generated model classes', 'Dismiss', { duration: 4000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not generate classes because ${error?.title}`, 'Dismiss', { duration: 4000 });
        },
      });
  }

  private deleteNode(node: ProjectFileNode) {
    this.isDeleteLoading.set(true);
    this.projectStructureService
      .deleteProjectStructureNode(this.projectId(), node.id)
      .pipe(finalize(() => this.isDeleteLoading.set(false)))
      .subscribe({
        next: () => {
          this.lazyTree().removeNode(node.id);
        },
      });
  }

  private renameFile(node: ProjectFile) {
    const dialogRef = this.dialog.open(RenameFileDialog, {
      disableClose: true,
      data: { projectId: this.projectId(), scriptId: node.id, currentScriptName: node.name },
    });

    dialogRef.afterClosed().subscribe((newName: string | undefined) => {
      if (!newName || newName === node.name) {
        return;
      }

      this.projectStructureService.updateProjectScript(this.projectId(), node.id, newName, undefined).subscribe({
        next: (updatedScript) => {
          this.lazyTree().updateNode(node.id, (oldNode) => ({
            ...oldNode,
            data: {
              ...oldNode.data,
              name: updatedScript.name,
            },
          }));
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not rename script because ${error?.title}`, 'Dismiss', { duration: 4000 });
        },
      });
    });
  }

  onNodeClick(node: TreeNode<ProjectFileNode>) {
    if (node.data.type === 'file') {
      this.fileSelected.emit(node.data);
    }
  }
}
