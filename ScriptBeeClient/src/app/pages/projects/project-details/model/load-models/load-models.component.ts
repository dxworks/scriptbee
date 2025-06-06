import { Component, input, signal } from '@angular/core';
import { TreeNode, TreeNodeWithParent } from '../../../../../types/tree-node';
import { MatButtonModule } from '@angular/material/button';
import { CheckableTreeComponent } from '../../../../../components/checkable-tree/checkable-tree.component';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { apiHandler } from '../../../../../utils/apiHandler';
import { LoaderService } from '../../../../../services/loaders/loader.service';

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss'],
  imports: [MatButtonModule, CheckableTreeComponent, CenteredSpinnerComponent, ErrorStateComponent],
})
export class LoadModelsComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();

  savedFiles = signal<TreeNode[]>([]);
  checkedFiles = signal<TreeNodeWithParent[]>([]);

  loadModelsHandler = apiHandler((params: { projectId: string; instanceId: string; checkedFiles: TreeNode[] }) =>
    // TODO FIXIT(#14): update with the list of loaders
    this.loaderService.loadModels(params.projectId, params.instanceId, params.checkedFiles)
  );

  constructor(private loaderService: LoaderService) {}

  onUpdateCheckedFiles(checkedNodes: TreeNodeWithParent[]) {
    this.checkedFiles.set(checkedNodes.filter((node) => !!node.parent));
  }

  onLoadFilesClick() {
    this.loadModelsHandler.execute({
      projectId: this.projectId(),
      instanceId: this.instanceId(),
      checkedFiles: this.checkedFiles(),
    });
  }
}
