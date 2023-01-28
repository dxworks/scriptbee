import { Component, Input } from '@angular/core';
import { Project } from "../../../state/project-details/project";
import { LoaderService } from "../../../services/loader/loader.service";
import { TreeNode } from "../../../shared/tree-node";
import { Store } from "@ngrx/store";
import { ErrorDialogService } from "../../../shared/error-dialog/error-dialog.service";
import { setLoadedModels } from "../../../state/project-details/project-details.actions";

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss']
})
export class LoadModelsComponent {

  @Input()
  project: Project | undefined;

  checkedFiles: TreeNode[] = [];
  loading = false;

  constructor(private store: Store, private errorDialogService: ErrorDialogService,
              private loaderService: LoaderService) {
  }

  onUpdateCheckedFiles(checkedNodes: TreeNode[]) {
    this.checkedFiles = checkedNodes;
  }

  onLoadFilesClick() {
    this.loading = true;
    const projectId = this.project.data.projectId;

    this.loaderService.loadModels(projectId, this.checkedFiles).subscribe({
      next: result => {
        for (const loadModelsResult of result) {
          this.store.dispatch(setLoadedModels({
            files:
            loadModelsResult.models, loader: loadModelsResult.name
          }))
        }

        this.loading = false;
      },
      error: err => {
        this.loading = false;
        this.errorDialogService.displayDialogErrorMessage('Could not load models', LoadModelsComponent.getErrorMessage(err));
      }
    })
  }

  // todo after errors are standardized, remove this
  private static getErrorMessage(error: any) {
    return error.error?.detail ?? error.error;
  }
}
