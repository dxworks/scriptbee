import { Injectable, Type } from '@angular/core';
import { AnalysisFile } from '../../types/analysis-results';
import { MonacoEditorViewerComponent } from './monaco-editor-viewer/monaco-editor-viewer.component';

export interface FileViewerPlugin {
  id: string;
  name: string;
  icon: string;
  component: Type<unknown>;
}

@Injectable({ providedIn: 'root' })
export class FileViewerService {
  private plugins: FileViewerPlugin[] = [
    {
      id: 'monaco-editor-default',
      name: 'Monaco Editor',
      icon: 'code',
      component: MonacoEditorViewerComponent,
    },
  ];

  getAvailablePluginsForFile(file: AnalysisFile): FileViewerPlugin[] {
    console.log(file);
    return this.plugins;
  }
}
