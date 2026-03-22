import { Component, input, signal } from '@angular/core';
import { TreeNode, TreeNodeWithParent } from '../../../../../types/tree-node';
import { MatButtonModule } from '@angular/material/button';
import { CheckableTreeComponent } from '../../../../../components/checkable-tree/checkable-tree.component';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { LoaderService } from '../../../../../services/loaders/loader.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-load-models',
  templateUrl: './load-models.component.html',
  styleUrls: ['./load-models.component.scss'],
  imports: [MatButtonModule, CheckableTreeComponent, CenteredSpinnerComponent],
})
export class LoadModelsComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();

  savedFiles = signal<TreeNode[]>([]);
  checkedFiles = signal<TreeNodeWithParent[]>([]);
  isLoadModelsLoading = signal(false);

  constructor(private loaderService: LoaderService) {}

  onUpdateCheckedFiles(checkedNodes: TreeNodeWithParent[]) {
    this.checkedFiles.set(checkedNodes.filter((node) => !!node.parent));
  }

  onLoadFilesClick() {
    this.isLoadModelsLoading.set(true);
    // TODO FIXIT(#14): update with the list of loaders
    this.loaderService
      .loadModels(this.projectId(), this.instanceId(), this.checkedFiles())
      .pipe(finalize(() => this.isLoadModelsLoading.set(false)))
      .subscribe();
  }
}
