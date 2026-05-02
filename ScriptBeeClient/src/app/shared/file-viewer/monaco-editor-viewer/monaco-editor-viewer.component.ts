import { Component, computed, inject, input } from '@angular/core';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { FormsModule } from '@angular/forms';
import { AnalysisFile } from '../../../types/analysis-results';
import { ThemeService } from '../../../services/common/theme.service';

@Component({
  selector: 'app-monaco-editor-viewer',
  templateUrl: './monaco-editor-viewer.component.html',
  styleUrls: ['./monaco-editor-viewer.component.scss'],
  imports: [EditorComponent, FormsModule],
})
export class MonacoEditorViewerComponent {
  content = input.required<string>();
  file = input.required<AnalysisFile>();

  private themeService = inject(ThemeService);

  editorOptions = computed(() => {
    const extension = this.file().name.split('.').pop()?.toLowerCase();
    let language = 'plaintext';

    switch (extension) {
      case 'json':
        language = 'json';
        break;
      case 'xml':
        language = 'xml';
        break;
      case 'csv':
        language = 'csv';
        break;
      case 'js':
        language = 'javascript';
        break;
      case 'ts':
        language = 'typescript';
        break;
      case 'html':
        language = 'html';
        break;
      case 'css':
        language = 'css';
        break;
    }

    return {
      theme: this.themeService.darkMode() ? 'vs-dark' : 'vs-light',
      language: language,
      readOnly: true,
      automaticLayout: true,
    };
  });
}
