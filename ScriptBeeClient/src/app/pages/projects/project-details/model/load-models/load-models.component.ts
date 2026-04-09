import { Component, computed, inject, input, signal } from '@angular/core';
import { TreeNode, TreeNodeWithParent } from '../../../../../types/tree-node';
import { MatButtonModule } from '@angular/material/button';
import { CheckableTreeComponent } from '../../../../../components/tree/checkable-tree/checkable-tree.component';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { LoaderService } from '../../../../../services/loaders/loader.service';
import { finalize } from 'rxjs';
import { Project, ProjectFile } from '../../../../../types/project';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { convertError } from '../../../../../utils/api';

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss'],
  imports: [MatButtonModule, CheckableTreeComponent, CenteredSpinnerComponent],
})
export class LoadModelsComponent {
  project = input.required<Project>();
  instanceId = input.required<string>();

  savedFiles = computed<TreeNode<ProjectFile | null>[]>(() => {
    const project = this.project();

    const nodes: TreeNode<ProjectFile | null>[] = [];
    for (const loaderId of Object.keys(project.savedFiles)) {
      nodes.push({
        name: loaderId,
        data: null,
        children: project.savedFiles[loaderId].map((file) => ({ name: file.name, data: file })),
      });
    }
    return nodes;
  });

  checkedFiles = signal<TreeNodeWithParent<ProjectFile | null>[]>([]);
  isLoadModelsLoading = signal(false);

  private loaderService = inject(LoaderService);
  private snackbar = inject(MatSnackBar);

  onUpdateCheckedFiles(checkedNodes: TreeNodeWithParent<ProjectFile | null>[]) {
    this.checkedFiles.set(checkedNodes.filter((node) => !!node.parent));
  }

  onLoadFilesClick() {
    this.isLoadModelsLoading.set(true);

    const loaderIds = this.checkedFiles().map((node) => node.parent!.name);

    this.loaderService
      .loadModels(this.project().id, this.instanceId(), loaderIds)
      .pipe(finalize(() => this.isLoadModelsLoading.set(false)))
      .subscribe({
        next: () => {
          this.snackbar.open('Load successful', 'Dismiss', { duration: 4000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not load models because ${error?.title}`, 'Dismiss', { duration: 4000 });
        },
      });
  }
}
