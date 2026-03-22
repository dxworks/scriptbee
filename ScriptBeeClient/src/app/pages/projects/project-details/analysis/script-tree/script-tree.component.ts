import { Component, computed, input, output, signal } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatTooltip } from '@angular/material/tooltip';
import { MatIcon } from '@angular/material/icon';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { TreeNode } from '../../../../../types/tree-node';
import { MatDialog } from '@angular/material/dialog';
import { CreateScriptDialogComponent } from './create-script-dialog/create-script-dialog.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { rxResource } from '@angular/core/rxjs-interop';
import { ProjectStructureService } from '../../../../../services/projects/project-structure.service';
import { finalize } from 'rxjs';
import { convertError } from '../../../../../utils/api';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  imports: [MatIconButton, MatTooltip, MatIcon, SelectableTreeComponent, ErrorStateComponent, LoadingProgressBarComponent],
})
export class ScriptTreeComponent {
  projectId = input.required<string>();

  onFileSelected = output<TreeNode>();

  projectStructureResource = rxResource({
    params: () => this.projectId(),
    stream: ({ params: projectId }) => this.projectStructureService.getProjectStructure(projectId),
  });
  projectStructureResourceError = computed(() => convertError(this.projectStructureResource.error()));

  isDeleteLoading = signal(false);

  constructor(
    private projectStructureService: ProjectStructureService,
    private dialog: MatDialog
  ) {}

  onCreateNewScriptButtonClick() {
    this.dialog.open(CreateScriptDialogComponent, {
      disableClose: true,
      data: { projectId: this.projectId() },
    });
  }

  onNodeDelete(node: TreeNode) {
    this.isDeleteLoading.set(true);
    this.projectStructureService
      .deleteProjectStructureNode(this.projectId(), node.name)
      .pipe(finalize(() => this.isDeleteLoading.set(false)))
      .subscribe({ next: () => this.projectStructureResource.reload() });
  }

  onNodeClick(node: TreeNode) {
    this.onFileSelected.emit(node);
  }
}
