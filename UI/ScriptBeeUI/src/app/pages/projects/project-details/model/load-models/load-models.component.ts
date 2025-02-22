import { Component, input, signal } from '@angular/core';
import { TreeNode, TreeNodeWithParent } from '../../../../../types/tree-node';
import { MatButtonModule } from '@angular/material/button';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { apiHandler } from '../../../../../utils/apiHandler';
import { LoaderService } from '../../../../../services/loaders/loader.service';

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss'],
  imports: [MatButtonModule, SelectableTreeComponent, CenteredSpinnerComponent, ErrorStateComponent],
})
export class LoadModelsComponent {
  projectId = input.required<string>();

  savedFiles = signal<TreeNode[]>([]);
  checkedFiles = signal<TreeNodeWithParent[]>([]);

  loadModelsHandler = apiHandler(
    (params: { projectId: string; checkedFiles: TreeNode[] }) => this.loaderService.loadModels(params.projectId, params.checkedFiles),
    (data) => {
      console.log(data);
    }
  );

  constructor(private loaderService: LoaderService) {}

  onUpdateCheckedFiles(checkedNodes: TreeNodeWithParent[]) {
    this.checkedFiles.set(checkedNodes.filter((node) => !!node.parent));
  }

  onLoadFilesClick() {
    this.loadModelsHandler.execute({
      projectId: this.projectId(),
      checkedFiles: this.checkedFiles(),
    });
  }
}
