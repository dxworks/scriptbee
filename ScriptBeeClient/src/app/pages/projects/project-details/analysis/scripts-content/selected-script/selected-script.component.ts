import { Component, computed, input } from '@angular/core';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '../../../../../../services/theme/theme.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SelectedScriptActionBarComponent } from './selected-script-action-bar/selected-script-action-bar.component';
import { createRxResourceHandler } from '../../../../../../utils/resource';
import { ProjectStructureService } from '../../../../../../services/projects/project-structure.service';
import { ErrorStateComponent } from '../../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../../components/loading-progress-bar/loading-progress-bar.component';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss'],
  imports: [EditorComponent, MatButtonModule, MatIconModule, FormsModule, SelectedScriptActionBarComponent, ErrorStateComponent, LoadingProgressBarComponent],
})
export class SelectedScriptComponent {
  projectId = input.required<string>();
  scriptId = input.required<string>();

  editorOptions = computed(() => {
    const language = this.scriptResource.value()?.scriptLanguage.name ?? 'csharp';

    return {
      theme: this.themeService.darkMode() ? 'vs-dark' : 'vs-light',
      language: language,
      readOnly: true,
      automaticLayout: true,
    };
  });

  scriptResource = createRxResourceHandler({
    request: () => ({
      projectId: this.projectId(),
      scriptId: this.scriptId(),
    }),
    loader: (params) => this.projectStructureService.getProjectScript(params.request.projectId, params.request.scriptId),
  });

  scriptContentResource = createRxResourceHandler({
    request: () => ({
      projectId: this.projectId(),
      scriptId: this.scriptId(),
    }),
    loader: (params) => this.projectStructureService.getScriptContent(params.request.projectId, params.request.scriptId),
  });

  constructor(
    private themeService: ThemeService,
    private projectStructureService: ProjectStructureService
  ) {}
}
