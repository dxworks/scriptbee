import { inject, Injectable, Type } from '@angular/core';
import { AnalysisFile } from '../../types/analysis-results';
import { MonacoEditorViewerComponent } from './monaco-editor-viewer/monaco-editor-viewer.component';
import { GatewayPluginsService } from '../../services/plugin/gateway-plugins.service';
import { FilePreviewExtensionPointOutlet } from '../../types/plugin';
import { RemotePluginHostComponent } from './remote-plugin-host/remote-plugin-host.component';

export interface FileViewerPlugin {
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

  private plugins: FileViewerPlugin[] = [
    {
      id: 'monaco-editor-default',
      name: 'Monaco Editor',
      icon: 'code',
      component: MonacoEditorViewerComponent,
      supportedFileExtensions: undefined,
    },
  ];

  getAvailablePluginsForFile(file: AnalysisFile): FileViewerPlugin[] {
    const fileViewerPlugins: FileViewerPlugin[] = this.gatewayPluginsService.filePreviewOutlets().map((outlet) => ({
      id: outlet.exposedModule,
      name: outlet.label,
      icon: outlet.icon,
      supportedFileExtensions: outlet.supportedFileExtensions,
      component: RemotePluginHostComponent,
      pluginOutlet: outlet,
    }));

    const extension = file.name.split('.').pop()?.toLowerCase();
    const allPlugins = [...this.plugins, ...fileViewerPlugins];

    if (!extension) {
      return allPlugins;
    }

    return allPlugins.filter((plugin) => {
      return !plugin.supportedFileExtensions || plugin.supportedFileExtensions.includes(extension);
    });
  }
}
