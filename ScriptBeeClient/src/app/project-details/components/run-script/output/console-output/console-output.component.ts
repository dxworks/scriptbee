import { Component, Input } from '@angular/core';
import { OutputFilesService } from '../../../../../services/output/output-files.service';

@Component({
  selector: 'app-console-output',
  templateUrl: './console-output.component.html',
  styleUrls: ['./console-output.component.scss']
})
export class ConsoleOutputComponent {
  // todo add the possibility for realtime updates

  @Input() set outputId(outputId: string | undefined) {
    if (!outputId) {
      this.consoleOutput = '';
      return;
    }

    this.outputFilesService.fetchOutput(outputId).subscribe((output) => {
      this.consoleOutput = output;
    });
  }

  consoleOutput = '';

  constructor(private outputFilesService: OutputFilesService) {}
}
