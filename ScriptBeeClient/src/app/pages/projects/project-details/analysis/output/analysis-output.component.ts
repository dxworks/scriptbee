import { Component, input } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { ConsoleOutputComponent } from './console-output/console-output.component';
import { OutputErrorsComponent } from './output-errors/output-errors.component';
import { FileOutputComponent } from './file-output/file-output.component';
import { OutputFilesService } from '../../../../../services/output/output-files.service';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';

@Component({
  selector: 'app-analysis-output',
  imports: [MatTabsModule, ConsoleOutputComponent, OutputErrorsComponent, FileOutputComponent, ErrorStateComponent, LoadingProgressBarComponent],
  templateUrl: './analysis-output.component.html',
  styleUrl: './analysis-output.component.scss',
})
export class AnalysisOutputComponent {
  projectId = input.required<string>();
  analysisId = input.required<string>();

  consoleContentResource = createRxResourceHandler({
    request: () => ({
      projectId: this.projectId(),
      analysisId: this.analysisId(),
    }),
    loader: (params) => this.outputFilesService.getConsoleOutput(params.request.projectId, params.request.analysisId),
  });

  outputErrorsResource = createRxResourceHandler({
    request: () => ({
      projectId: this.projectId(),
      analysisId: this.analysisId(),
    }),
    loader: (params) => this.outputFilesService.getErrorOutputs(params.request.projectId, params.request.analysisId),
  });

  outputFilesResource = createRxResourceHandler({
    request: () => ({
      projectId: this.projectId(),
      analysisId: this.analysisId(),
    }),
    loader: (params) => this.outputFilesService.getFileOutputs(params.request.projectId, params.request.analysisId),
  });

  constructor(private outputFilesService: OutputFilesService) {}
}
