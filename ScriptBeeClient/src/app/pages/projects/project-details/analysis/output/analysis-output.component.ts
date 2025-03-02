import { Component, input, signal } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { ConsoleOutputComponent } from './console-output/console-output.component';
import { OutputErrorsComponent } from './output-errors/output-errors.component';
import { FileOutputComponent } from './file-output/file-output.component';
import { AnalysisFile, AnalysisRunError } from '../../../../../types/analysis-results';

@Component({
  selector: 'app-analysis-output',
  imports: [MatTabsModule, ConsoleOutputComponent, OutputErrorsComponent, FileOutputComponent],
  templateUrl: './analysis-output.component.html',
  styleUrl: './analysis-output.component.scss',
})
export class AnalysisOutputComponent {
  projectId = input.required<string>();
  analysisId = input.required<string>();

  consoleContent = signal('test');
  outputErrors = signal<AnalysisRunError[]>([
    {
      title: 'Error',
      message: 'Some error on line 1',
      severity: 'error',
    },
    {
      title: 'Other Error',
      message: 'Some error on line 1',
      severity: 'error',
    },
  ]);
  files = signal<AnalysisFile[]>([
    {
      id: '1',
      name: 'file.csv',
      type: 'file',
    },
    {
      id: '2',
      name: 'file.csv',
      type: 'file',
    },
    {
      id: '3',
      name: 'file.csv',
      type: 'file',
    },
  ]);

  // TODO FIXIT: use the output service and replace hardcoded values
  // TODO FIXIT: add loading and error handling
}
