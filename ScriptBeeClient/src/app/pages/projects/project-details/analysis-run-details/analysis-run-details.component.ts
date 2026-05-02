import { Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { AnalysisService } from '../../../../services/analysis/analysis.service';
import { ProjectStructureService } from '../../../../services/projects/project-structure.service';
import { ThemeService } from '../../../../services/common/theme.service';
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
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { convertError } from '../../../../utils/api';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../../../components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProjectStateService } from '../../../../services/projects/project-state.service';

@Component({
  selector: 'app-analysis-run-details',
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
    MatDialogModule,
    MatSnackBarModule,
  ],
  templateUrl: './analysis-run-details.component.html',
  styleUrls: ['./analysis-run-details.component.scss'],
})
export class AnalysisRunDetailsComponent {
  project = computed(() => this.projectStateService.currentProject()!);
  analysisId = input.required<string>();

  private projectStateService = inject(ProjectStateService);
  private analysisService = inject(AnalysisService);
  private projectStructureService = inject(ProjectStructureService);
  private themeService = inject(ThemeService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  showDiff = signal<boolean>(false);
  isDeleting = signal<boolean>(false);

  runResource = rxResource({
    params: () => {
      return { projectId: this.project().id, analysisId: this.analysisId() };
    },
    stream: ({ params }) => {
      if (!params.projectId || !params.analysisId) {
        return of(null);
      }

      return this.analysisService.getAnalysis(params.projectId, params.analysisId).pipe(
        switchMap((analysis) => {
          return forkJoin({
            analysis: of(analysis),
            content: this.analysisService.getAnalysisScriptContent(params.projectId, params.analysisId, analysis.scriptId).pipe(catchError(() => of(''))),
            metadata: this.analysisService.getAnalysisScriptMetadata(params.projectId, params.analysisId, analysis.scriptId).pipe(catchError(() => of(null))),
            current: this.projectStructureService.getProjectScript(params.projectId, analysis.scriptId).pipe(
              switchMap((script) => {
                if (!script) {
                  return of(null);
                }
                return this.projectStructureService.getScriptContent(params.projectId, analysis.scriptId).pipe(
                  map((content) => {
                    return { script, content };
                  }),
                  catchError(() => of(null))
                );
              }),
              catchError(() => of(null))
            ),
          });
        })
      );
    },
  });
  error = computed(() => convertError(this.runResource.error()));

  onDeleteClick() {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Delete Analysis',
        message: 'Are you sure you want to delete this analysis and all its results? This action cannot be undone.',
        confirmText: 'Delete',
        cancelText: 'Cancel',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        const projectId = this.project().id;
        this.isDeleting.set(true);
        this.analysisService.deleteAnalysis(projectId, this.analysisId()).subscribe({
          next: () => {
            this.isDeleting.set(false);
            this.snackBar.open('Analysis deleted successfully', 'Close', { duration: 3000 });
            void this.router.navigate(['/projects', projectId, 'analysis']);
          },
          error: (err) => {
            this.isDeleting.set(false);
            this.snackBar.open(`Failed to delete analysis: ${convertError(err)}`, 'Close', { duration: 5000 });
          },
        });
      }
    });
  }

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
