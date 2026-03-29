import { Component, computed, input, signal } from '@angular/core';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '../../../../../../services/theme/theme.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SelectedScriptActionBarComponent } from './selected-script-action-bar/selected-script-action-bar.component';
import { rxResource } from '@angular/core/rxjs-interop';
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

  editorOptions = computed(() => {
    const language = this.scriptResource.value()?.scriptLanguage.name ?? 'csharp';

    return {
      theme: this.themeService.darkMode() ? 'vs-dark' : 'vs-light',
      language: language,
      readOnly: true,
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

  constructor(
    private themeService: ThemeService,
    private projectStructureService: ProjectStructureService
  ) {}

  protected onStatusUrlChanged(statusUrl: string | undefined) {
    this.statusUrl.set(statusUrl);
  }
}
