import { Component, inject, OnInit, signal, Type } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { AnalysisFile } from '../../../types/analysis-results';
import { OutputFilesService } from '../../../services/analysis/output-files.service';
import { MatButtonModule } from '@angular/material/button';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../components/error-state/error-state.component';
import { NgComponentOutlet } from '@angular/common';
import { ErrorResponse } from '../../../types/api';
import { convertError } from '../../../utils/api';
import { from, switchMap } from 'rxjs';

export interface AnalysisFilePreviewDialogData {
  projectId: string;
  analysisId: string;
  file: AnalysisFile;
  pluginComponent: Type<unknown>;
}

@Component({
  selector: 'app-analysis-file-preview-dialog',
  templateUrl: './analysis-file-preview-dialog.component.html',
  styleUrls: ['./analysis-file-preview-dialog.component.scss'],
  imports: [MatDialogModule, MatButtonModule, LoadingProgressBarComponent, ErrorStateComponent, NgComponentOutlet],
})
export class AnalysisFilePreviewDialogComponent implements OnInit {
  protected data = inject<AnalysisFilePreviewDialogData>(MAT_DIALOG_DATA);
  private outputFilesService = inject(OutputFilesService);

  protected fileContent = signal<string>('');
  protected isLoading = signal<boolean>(true);
  protected errorMessage = signal<ErrorResponse | undefined>(undefined);

  ngOnInit() {
    this.loadFileContent();
  }

  protected loadFileContent() {
    this.isLoading.set(true);
    this.errorMessage.set(undefined);

    this.outputFilesService
      .downloadFile(this.data.projectId, this.data.analysisId, this.data.file.id)
      .pipe(switchMap((blob) => from(blob.text())))
      .subscribe({
        next: (text) => {
          this.fileContent.set(text);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(convertError(err) || { title: 'Failed to download file.', status: 500 });
          this.isLoading.set(false);
        },
      });
  }
}
