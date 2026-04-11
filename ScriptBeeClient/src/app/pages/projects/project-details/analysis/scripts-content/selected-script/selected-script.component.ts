import { Component, computed, inject, input, output, signal } from '@angular/core';
import { rxResource, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, debounceTime, delay, of, Subject, switchMap, tap } from 'rxjs';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '../../../../../../services/theme/theme.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SelectedScriptActionBarComponent } from './selected-script-action-bar/selected-script-action-bar.component';
import { ProjectStructureService } from '../../../../../../services/projects/project-structure.service';
import { ErrorStateComponent } from '../../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { convertError } from '../../../../../../utils/api';
import { RunScriptLoadingComponent } from './run-script-loading/run-script-loading.component';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss'],
  imports: [
    EditorComponent,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    SelectedScriptActionBarComponent,
    ErrorStateComponent,
    LoadingProgressBarComponent,
    RunScriptLoadingComponent,
  ],
})
export class SelectedScriptComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();
  scriptId = input.required<string>();
  statusUrl = signal<string | undefined>(undefined);

  analysisFinished = output<string>();

  saveStatus = signal<'Saved' | 'Saving...' | 'Error' | undefined>(undefined);

  private themeService = inject(ThemeService);
  private projectStructureService = inject(ProjectStructureService);

  private updateContent$ = new Subject<string>();

  constructor() {
    this.updateContent$
      .pipe(
        debounceTime(1000),
        tap(() => this.saveStatus.set('Saving...')),
        switchMap((content) =>
          this.projectStructureService.updateScriptContent(this.projectId(), this.scriptId(), content).pipe(
            tap(() => this.saveStatus.set('Saved')),
            catchError(() => {
              this.saveStatus.set('Error');
              return of(null);
            }),
            delay(2000),
            tap(() => {
              if (this.saveStatus() === 'Saved') {
                this.saveStatus.set(undefined);
              }
            })
          )
        ),
        takeUntilDestroyed()
      )
      .subscribe();
  }

  editorOptions = computed(() => {
    const language = this.scriptResource.value()?.scriptLanguage.name ?? 'csharp';

    return {
      theme: this.themeService.darkMode() ? 'vs-dark' : 'vs-light',
      language: language,
      readOnly: false,
      automaticLayout: true,
    };
  });

  scriptResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      scriptId: this.scriptId(),
    }),
    stream: ({ params }) => this.projectStructureService.getProjectScript(params.projectId, params.scriptId),
  });
  scriptResourceError = computed(() => convertError(this.scriptResource.error()));

  scriptContentResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      scriptId: this.scriptId(),
    }),
    stream: ({ params }) => this.projectStructureService.getScriptContent(params.projectId, params.scriptId),
  });
  scriptContentResourceError = computed(() => convertError(this.scriptContentResource.error()));

  protected onStatusUrlChanged(statusUrl: string | undefined) {
    this.statusUrl.set(statusUrl);
  }

  protected onAnalysisFinished(analysisId: string) {
    this.analysisFinished.emit(analysisId);
  }

  protected onContentChange(newContent: string) {
    this.updateContent$.next(newContent);
  }
}
