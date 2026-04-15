import { Component, computed, inject, input } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { ConsoleOutputComponent } from './console-output/console-output.component';
import { OutputErrorsComponent } from './output-errors/output-errors.component';
import { FileOutputComponent } from './file-output/file-output.component';
import { OutputFilesService } from '../../../../../services/analysis/output-files.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { convertError } from '../../../../../utils/api';

@Component({
  selector: 'app-analysis-output',
  imports: [MatTabsModule, ConsoleOutputComponent, OutputErrorsComponent, FileOutputComponent, ErrorStateComponent, LoadingProgressBarComponent],
  templateUrl: './analysis-output.component.html',
  styleUrl: './analysis-output.component.scss',
})
export class AnalysisOutputComponent {
  projectId = input.required<string>();
  analysisId = input.required<string>();

  private outputFilesService = inject(OutputFilesService);

  consoleContentResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      analysisId: this.analysisId(),
    }),
    stream: ({ params }) => this.outputFilesService.getConsoleOutput(params.projectId, params.analysisId),
  });
  consoleContentResourceError = computed(() => convertError(this.consoleContentResource.error()));

  outputErrorsResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      analysisId: this.analysisId(),
    }),
    stream: ({ params }) => this.outputFilesService.getErrorOutputs(params.projectId, params.analysisId),
  });
  outputErrorsResourceError = computed(() => convertError(this.outputErrorsResource.error()));

  outputFilesResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      analysisId: this.analysisId(),
    }),
    stream: ({ params }) => this.outputFilesService.getFileOutputs(params.projectId, params.analysisId),
  });
  outputFilesResourceError = computed(() => convertError(this.outputFilesResource.error()));
}
