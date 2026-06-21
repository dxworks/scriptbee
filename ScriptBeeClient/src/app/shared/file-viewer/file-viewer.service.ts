import { inject, Injectable, Type } from '@angular/core';
import { AnalysisFile } from '../../types/analysis-results';
import { MonacoEditorViewerComponent } from './monaco-editor-viewer/monaco-editor-viewer.component';
import { GatewayPluginsService } from '../../services/plugin/gateway-plugins.service';
import { FilePreviewExtensionPointOutlet } from '../../types/plugin';
import { RemotePluginHostComponent } from './remote-plugin-host/remote-plugin-host.component';

export interface FilePreviewer {
  id: string;
  name: string;
  icon?: string;
  component: Type<unknown>;
  supportedFileExtensions: string[] | undefined;
  pluginOutlet?: FilePreviewExtensionPointOutlet;
}

@Injectable({ providedIn: 'root' })
export class FileViewerService {
  private gatewayPluginsService = inject(GatewayPluginsService);

  private defaultPreviewers: FilePreviewer[] = [
    {
      id: 'monaco-editor-default',
      name: 'Monaco Editor',
      icon: 'code',
      component: MonacoEditorViewerComponent,
      supportedFileExtensions: undefined,
      pluginOutlet: undefined,
    },
  ];

  getAvailablePreviewersForFile(file: AnalysisFile): FilePreviewer[] {
    const pluginPreviewers: FilePreviewer[] = this.gatewayPluginsService.filePreviewOutlets().map((outlet) => ({
      id: `${outlet.remoteEntry}-${outlet.exposedModule}`,
      name: outlet.label,
      icon: outlet.icon,
      supportedFileExtensions: outlet.supportedFileExtensions,
      component: RemotePluginHostComponent,
      pluginOutlet: outlet,
    }));

    const extension = file.name.split('.').pop()?.toLowerCase();
    const allPreviewers = [...this.defaultPreviewers, ...pluginPreviewers];

    if (!extension) {
      return allPreviewers;
    }

    return allPreviewers.filter((plugin) => {
      return !plugin.supportedFileExtensions || plugin.supportedFileExtensions.includes(extension);
    });
  }
}
