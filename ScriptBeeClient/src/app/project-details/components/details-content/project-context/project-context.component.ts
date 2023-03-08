import { Component, OnInit } from '@angular/core';
import { ErrorDialogService } from '../../../../shared/error-dialog/error-dialog.service';
import { ProjectStore } from '../../../stores/project-store.service';
import { ContextStore } from '../../../stores/context-store.service';
import { LoadersStore } from '../../../stores/loaders-store.service';

@Component({
  selector: 'app-project-context',
  templateUrl: './project-context.component.html',
  styleUrls: ['./project-context.component.scss'],
})
export class ProjectContextComponent implements OnInit {
  loadedFiles$ = this.loadersStore.loadedFiles;
  context$ = this.contextStore.context;
  contextLoading$ = this.contextStore.contextLoading;

  reloadingContext$ = this.contextStore.reloadingContext;
  clearingContext$ = this.contextStore.clearingContext;

  private projectId: string;

  constructor(
    private projectStore: ProjectStore,
    private contextStore: ContextStore,
    private loadersStore: LoadersStore,
    private errorDialogService: ErrorDialogService
  ) {}

  ngOnInit(): void {
    this.projectId = this.projectStore.getProjectId();

    this.contextStore.reloadingContextError.subscribe((error) => {
      if (error) {
        this.errorDialogService.displayDialogErrorMessage('Could not reload project context', error.message);
      }
    });

    this.contextStore.clearingContextError.subscribe((error) => {
      if (error) {
        this.errorDialogService.displayDialogErrorMessage('Could not clear project context', error.message);
      }
    });

    this.contextStore.contextError.subscribe((error) => {
      if (error) {
        this.errorDialogService.displayDialogErrorMessage('Could not load project context', error.message);
      }
    });

    this.contextStore.loadContext({ projectId: this.projectId });
  }

  onReloadModelsClick() {
    this.contextStore.reloadContext({ projectId: this.projectId });
  }

  onClearContextButtonClick() {
    this.contextStore.clearContext({ projectId: this.projectId });
  }
}
