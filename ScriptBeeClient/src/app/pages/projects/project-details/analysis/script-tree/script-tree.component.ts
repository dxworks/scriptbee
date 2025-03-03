import { Component, input, output } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatTooltip } from '@angular/material/tooltip';
import { MatIcon } from '@angular/material/icon';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { TreeNode } from '../../../../../types/tree-node';
import { MatDialog } from '@angular/material/dialog';
import { CreateScriptDialogComponent } from './create-script-dialog/create-script-dialog.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { ProjectStructureService } from '../../../../../services/projects/project-structure.service';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  imports: [MatIconButton, MatTooltip, MatIcon, SelectableTreeComponent, ErrorStateComponent, LoadingProgressBarComponent],
})
export class ScriptTreeComponent {
  projectId = input.required<string>();

  onFileSelected = output<TreeNode>();

  projectStructureResource = createRxResourceHandler({
    request: () => this.projectId(),
    loader: (params) => this.projectStructureService.getProjectStructure(params.request),
  });

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
    // TODO FIXIT: implement it
    console.log('delete node', node);
  }

  onNodeClick(node: TreeNode) {
    this.onFileSelected.emit(node);
  }
}
