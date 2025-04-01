import { Component, signal } from '@angular/core';
import { AngularSplitModule } from 'angular-split';
import { ScriptsContentComponent } from './scripts-content/scripts-content.component';
import { ScriptTreeComponent } from './script-tree/script-tree.component';
import { AnalysisOutputComponent } from './output/analysis-output.component';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TreeNode } from '../../../../types/tree-node';
import { InstanceService } from '../../../../services/instances/instance.service';

@Component({
  selector: 'app-analysis',
  templateUrl: './analysis.component.html',
  styleUrls: ['./analysis.component.scss'],
  imports: [AngularSplitModule, ScriptsContentComponent, ScriptTreeComponent, AnalysisOutputComponent],
})
export class AnalysisComponent {
  projectId = signal<string | undefined>(undefined);
  analysisId = signal<string | undefined>(undefined);
  instanceId = signal<string | undefined>(undefined);

  selectedFileId = signal<string | null>(null);

  // TODO: add the possibility to select the analysis (analysis should have also runIndex to be displayed to the user)
  // TODO FIXIT: select the last analysis by default

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private instanceService: InstanceService
  ) {
    route.queryParamMap.subscribe((params) => {
      this.selectedFileId.set(params.get('fileId'));
    });

    route.parent?.paramMap.pipe(takeUntilDestroyed()).subscribe({
      next: (paramMap) => {
        const id = paramMap.get('id');
        this.projectId.set(id ?? undefined);

        if (id) {
          this.instanceService.getCurrentInstance(id).subscribe({
            next: (instanceInfo) => {
              this.instanceId.set(instanceInfo.id);
            },
          });
        }
      },
    });
  }

  onFileSelected(node: TreeNode) {
    this.router
      .navigate([], {
        relativeTo: this.route,
        queryParams: { fileId: node.name },
        queryParamsHandling: 'merge',
      })
      .then();
  }
}
