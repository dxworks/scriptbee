import { Component, Input } from '@angular/core';
import { Project } from "../../../state/project-details/project";
import { Store } from "@ngrx/store";
import { ErrorDialogService } from "../../../shared/error-dialog/error-dialog.service";
import { LoaderService } from "../../../services/loader/loader.service";
import { clearContext, setLoadedModels } from "../../../state/project-details/project-details.actions";

@Component({
  selector: 'app-project-context',
  templateUrl: './project-context.component.html',
  styleUrls: ['./project-context.component.scss']
})
export class ProjectContextComponent {

  @Input()
  project: Project | undefined;
  reloadingContext = false;
  clearingContext = false;

  constructor(private store: Store, private errorDialogService: ErrorDialogService,
              private loaderService: LoaderService) {
  }

  onReloadModelsClick() {
    this.reloadingContext = true;
    const projectId = this.project.data.projectId;

    this.loaderService.reloadProjectContext(projectId).subscribe({
      next: result => {
        for (const loadModelsResult of result) {
          this.store.dispatch(setLoadedModels({
            files:
            loadModelsResult.models, loader: loadModelsResult.name
          }))
        }

        this.reloadingContext = false;
      },
      error: (error) => {
        this.reloadingContext = false;
        this.errorDialogService.displayDialogErrorMessage('Could not reload project context', ProjectContextComponent.getErrorMessage(error));
      }
    });
  }

  onClearContextButtonClick() {
    this.clearingContext = true;
    const projectId = this.project.data.projectId;

    this.loaderService.clearProjectContext(projectId).subscribe({
      next: () => {
        this.clearingContext = false;
        this.store.dispatch(clearContext())
      },
      error: (err) => {
        this.clearingContext = false;
        this.errorDialogService.displayDialogErrorMessage('Could not clear project context', ProjectContextComponent.getErrorMessage(err));
      }
    })
  }

  private static getErrorMessage(error: any) {
    return error.error?.detail ?? error.error;
  }
}
