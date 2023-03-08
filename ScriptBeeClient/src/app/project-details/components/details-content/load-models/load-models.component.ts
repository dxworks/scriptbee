import { Component, OnInit } from '@angular/core';
import { TreeNode } from '../../../../shared/tree-node';
import { ErrorDialogService } from '../../../../shared/error-dialog/error-dialog.service';
import { LoadersStore } from '../../../stores/loaders-store.service';
import { ProjectStore } from '../../../stores/project-store.service';
import { ContextStore } from '../../../stores/context-store.service';

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss'],
})
export class LoadModelsComponent implements OnInit {
  loadedFiles$ = this.loadersStore.savedFiles;
  loadModelsLoading$ = this.loadersStore.loadModelsLoading;

  checkedFiles: TreeNode[] = [];

  constructor(
    private projectStore: ProjectStore,
    private loadersStore: LoadersStore,
    private contextStore: ContextStore,
    private errorDialogService: ErrorDialogService
  ) {}

  ngOnInit(): void {
    this.loadersStore.loadModelsError.subscribe((error) => {
      if (error) {
        this.errorDialogService.displayDialogErrorMessage('Could not load models', error.message);
      }
    });
  }

  onUpdateCheckedFiles(checkedNodes: TreeNode[]) {
    this.checkedFiles = checkedNodes;
  }

  onLoadFilesClick() {
    const projectId = this.projectStore.getProjectId();

    this.loadersStore.loadModels({ projectId, models: this.checkedFiles });
    this.contextStore.loadContext({ projectId });
  }
}
