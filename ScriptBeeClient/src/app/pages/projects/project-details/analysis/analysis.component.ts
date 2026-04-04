import { Component, computed, model, signal, viewChild } from '@angular/core';
import { AngularSplitModule } from 'angular-split';
import { ScriptsContentComponent } from './scripts-content/scripts-content.component';
import { ScriptTreeComponent } from './script-tree/script-tree.component';
import { AnalysisOutputComponent } from './output/analysis-output.component';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AnalysisSelectorComponent } from '../../../../components/analysis-selector/analysis-selector.component';
import { ProjectFileNode } from '../../../../types/project';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-analysis',
  templateUrl: './analysis.component.html',
  styleUrls: ['./analysis.component.scss'],
  imports: [AngularSplitModule, ScriptsContentComponent, ScriptTreeComponent, AnalysisOutputComponent, AnalysisSelectorComponent, MatProgressBarModule],
})
export class AnalysisComponent {
  selectedFileId = signal<string | null>(null);
  analysisId = model<string | undefined>(undefined);

  selector = viewChild.required<AnalysisSelectorComponent>('selector');

  projectId = computed(() => this.projectStateService.currentProjectId());
  instanceId = computed(() => this.projectStateService.currentInstanceId());

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectStateService: ProjectStateService
  ) {
    route.queryParamMap.pipe(takeUntilDestroyed()).subscribe((params) => {
      this.selectedFileId.set(params.get('fileId'));
    });
  }

  onFileSelected(node: ProjectFileNode) {
    this.router
      .navigate([], {
        relativeTo: this.route,
        queryParams: { fileId: node.id },
        queryParamsHandling: 'merge',
      })
      .then();
  }

  onAnalysisFinished(analysisId: string) {
    this.selector().reload();
    this.analysisId.set(analysisId);
  }
}
