import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { AnalysisService } from '../../../../services/analysis/analysis.service';
import { ProjectStructureService } from '../../../../services/projects/project-structure.service';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { ThemeService } from '../../../../services/theme/theme.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { AnalysisOutputComponent } from '../analysis/output/analysis-output.component';
import { DiffEditorComponent, EditorComponent } from 'ngx-monaco-editor-v2';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { forkJoin, map, of, switchMap } from 'rxjs';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { convertError } from '../../../../utils/api';

@Component({
  selector: 'app-analysis-run-details',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    MatTooltipModule,
    MatChipsModule,
    AnalysisOutputComponent,
    EditorComponent,
    DiffEditorComponent,
    LoadingProgressBarComponent,
    ErrorStateComponent,
    FormsModule,
    DatePipe,
    RouterLink,
  ],
  templateUrl: './analysis-run-details.component.html',
  styleUrls: ['./analysis-run-details.component.scss'],
})
export class AnalysisRunDetailsComponent {
  private route = inject(ActivatedRoute);
  private analysisService = inject(AnalysisService);
  private projectStructureService = inject(ProjectStructureService);
  private projectStateService = inject(ProjectStateService);
  private themeService = inject(ThemeService);

  private routeParams = toSignal(this.route.paramMap);

  analysisId = computed(() => {
    const id = this.routeParams()?.get('analysisId');
    return id ?? '';
  });
  projectId = computed(() => this.projectStateService.currentProjectId()!);

  showDiff = signal<boolean>(false);

  runResource = rxResource({
    params: () => {
      return { projectId: this.projectId(), analysisId: this.analysisId() };
    },
    stream: ({ params }) => {
      if (!params.projectId || !params.analysisId) {
        return of(null);
      }

      return this.analysisService.getAnalysis(params.projectId, params.analysisId).pipe(
        switchMap((analysis) => {
          return forkJoin({
            analysis: of(analysis),
            content: this.analysisService.getAnalysisScriptContent(params.projectId, params.analysisId, analysis.scriptId),
            metadata: this.analysisService.getAnalysisScriptMetadata(params.projectId, params.analysisId, analysis.scriptId),
            current: this.projectStructureService.getProjectScript(params.projectId, analysis.scriptId).pipe(
              switchMap((script) => {
                if (!script) {
                  return of(null);
                }
                return this.projectStructureService.getScriptContent(params.projectId, analysis.scriptId).pipe(
                  map((content) => {
                    return { script, content };
                  })
                );
              })
            ),
          });
        })
      );
    },
  });
  error = computed(() => convertError(this.runResource.error()));

  creationDate = computed(() => {
    return this.runResource.value()?.analysis?.creationDate ?? null;
  });

  language = computed(() => {
    const data = this.runResource.value();
    if (data?.metadata?.scriptLanguage.name) {
      return data.metadata.scriptLanguage.name.toLowerCase();
    }
    if (data?.current?.script?.scriptLanguage.name) {
      return data.current.script.scriptLanguage.name.toLowerCase();
    }
    return 'plaintext';
  });

  originalScriptContent = computed(() => {
    return this.runResource.value()?.content ?? '';
  });

  currentScriptContent = computed(() => {
    return this.runResource.value()?.current?.content ?? '';
  });

  canShowDiff = computed(() => {
    return !!this.runResource.value()?.current;
  });

  scriptComparisonTitle = computed(() => {
    if (this.showDiff() && this.canShowDiff()) {
      return 'Comparison: Historical Analysis Script vs. Current Project Script';
    }
    return 'Analysis Script Content';
  });

  editorOptions = computed(() => {
    return {
      theme: this.themeService.darkMode() ? 'vs-dark' : 'vs-light',
      language: this.language(),
      readOnly: true,
      minimap: { enabled: false },
      scrollBeyondLastLine: false,
      automaticLayout: true,
    };
  });

  originalModel = computed(() => {
    return {
      code: this.originalScriptContent(),
      language: this.language(),
    };
  });

  modifiedModel = computed(() => {
    return {
      code: this.currentScriptContent(),
      language: this.language(),
    };
  });

  onRetry() {
    this.runResource.reload();
  }
}
