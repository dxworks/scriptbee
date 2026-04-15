import { Component, inject, input } from '@angular/core';
import { AnalysisFile } from '../../../../../../types/analysis-results';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { OutputFilesService } from '../../../../../../services/analysis/output-files.service';
import { DownloadService } from '../../../../../../services/common/download.service';

@Component({
  selector: 'app-file-output',
  templateUrl: './file-output.component.html',
  styleUrls: ['./file-output.component.scss'],
  imports: [MatIcon, MatButton],
})
export class FileOutputComponent {
  projectId = input.required<string>();
  analysisId = input.required<string>();

  files = input.required<AnalysisFile[]>();

  private outputFilesService = inject(OutputFilesService);
  private downloadService = inject(DownloadService);

  onDownloadFileButtonClick(file: AnalysisFile) {
    this.outputFilesService.downloadFile(this.projectId(), this.analysisId(), file.id).subscribe((data) => {
      this.downloadService.downloadFile(file.name, data);
    });
  }

  onDownloadAllButtonClick() {
    this.outputFilesService.downloadAll(this.projectId(), this.analysisId()).subscribe((data) => {
      this.downloadService.downloadFile('outputFiles.zip', data);
    });
  }
}
