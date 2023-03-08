import { Component, OnInit } from '@angular/core';
import { ErrorDialogService } from '../../../../shared/error-dialog/error-dialog.service';
import { ProjectStore } from '../../../stores/project-store.service';
import { LinkersStore } from '../../../stores/linkers-store.service';
import { ContextStore } from '../../../stores/context-store.service';

@Component({
  selector: 'app-link-models',
  templateUrl: './link-models.component.html',
  styleUrls: ['./link-models.component.scss'],
})
export class LinkModelsComponent implements OnInit {
  linkers$ = this.linkersStore.linkers;
  linkersLoading$ = this.linkersStore.linkersLoading;
  linkersError$ = this.linkersStore.linkersError;

  linkModelsLoading$ = this.linkersStore.linkModelsLoading;

  selectedLinker: string | undefined;
  private projectId: string;

  constructor(
    private projectStore: ProjectStore,
    private linkersStore: LinkersStore,
    private contextStore: ContextStore,
    private errorDialogService: ErrorDialogService
  ) {}

  ngOnInit(): void {
    this.projectId = this.projectStore.getProjectId();

    this.linkersStore.loadLinkers();

    this.linkersStore.linkModelsError.subscribe((error) => {
      if (error) {
        this.errorDialogService.displayDialogErrorMessage('Could not link models', error.message);
      }
    });
  }

  onLinkButtonClick() {
    if (!this.selectedLinker) {
      this.errorDialogService.displayDialogErrorMessage('You must select a linker first', '');
      return;
    }

    this.linkersStore.linkModels({ projectId: this.projectId, linkerName: this.selectedLinker });
  }
}
