import { Component, signal } from '@angular/core';
import { AngularSplitModule } from 'angular-split';
import { ScriptsContentComponent } from './scripts-content/scripts-content.component';
import { ScriptTreeComponent } from './script-tree/script-tree.component';
import { AnalysisOutputComponent } from './output/analysis-output.component';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-analysis',
  templateUrl: './analysis.component.html',
  styleUrls: ['./analysis.component.scss'],
  imports: [AngularSplitModule, ScriptsContentComponent, ScriptTreeComponent, AnalysisOutputComponent],
})
export class AnalysisComponent {
  projectId = signal<string | undefined>(undefined);
  analysisId = signal<string | undefined>(undefined);

  // TODO FIXIT: handle loading
  // TODO FIXIT: add the possibility to select the analysis (analysis should have also runIndex to be displayed to the user)
  // TODO FIXIT: select the last analysis by default

  constructor(route: ActivatedRoute) {
    route.queryParamMap.subscribe((params) => {
      console.log('queryParamMap', params);
    });

    route.parent?.paramMap.pipe(takeUntilDestroyed()).subscribe({
      next: (paramMap) => {
        this.projectId.set(paramMap.get('id') ?? undefined);
      },
    });
  }
}
