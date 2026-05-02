import { Component, inject, input } from '@angular/core';
import { AnalysisFile } from '../../../../../../types/analysis-results';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { OutputFilesService } from '../../../../../../services/analysis/output-files.service';
import { DownloadService } from '../../../../../../services/common/download.service';
import { FileViewerService, FileViewerPlugin } from '../../../../../../shared/file-viewer/file-viewer.service';
import { AnalysisFilePreviewDialogComponent } from '../../../../../../shared/file-viewer/analysis-file-preview-dialog/analysis-file-preview-dialog.component';

@Component({
  selector: 'app-file-output',
  templateUrl: './file-output.component.html',
  styleUrls: ['./file-output.component.scss'],
  imports: [MatIcon, MatButton, MatIconButton, MatMenuModule],
})
export class FileOutputComponent {
  projectId = input.required<string>();
  analysisId = input.required<string>();

  files = input.required<AnalysisFile[]>();

  private outputFilesService = inject(OutputFilesService);
  private downloadService = inject(DownloadService);
  public fileViewerService = inject(FileViewerService);
  private dialog = inject(MatDialog);

  onDownloadFileButtonClick(file: AnalysisFile, event: Event) {
    event.stopPropagation();
    this.outputFilesService.downloadFile(this.projectId(), this.analysisId(), file.id).subscribe((data: Blob) => {
      this.downloadService.downloadFile(file.name, data);
    });
  }

  onDownloadAllButtonClick() {
    this.outputFilesService.downloadAll(this.projectId(), this.analysisId()).subscribe((data: Blob) => {
      this.downloadService.downloadFile('outputFiles.zip', data);
    });
  }

  onPreviewFileClick(file: AnalysisFile) {
    const plugins = this.fileViewerService.getAvailablePluginsForFile(file);
    if (plugins.length === 1) {
      this.openWithPlugin(plugins[0], file);
    }
  }

  openWithPlugin(plugin: FileViewerPlugin, file: AnalysisFile) {
    this.dialog.open(AnalysisFilePreviewDialogComponent, {
      data: {
        projectId: this.projectId(),
        analysisId: this.analysisId(),
        file,
        pluginComponent: plugin.component,
      },
      width: '90vw',
      height: '90vh',
      maxWidth: '100vw',
      maxHeight: '100vh',
    });
  }
}
