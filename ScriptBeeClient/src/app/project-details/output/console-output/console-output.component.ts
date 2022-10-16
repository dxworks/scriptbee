import { Component, Input } from '@angular/core';
import { OutputFilesService } from "../../../services/output/output-files.service";

@Component({
  selector: 'app-console-output',
  templateUrl: './console-output.component.html',
  styleUrls: ['./console-output.component.scss']
})
export class ConsoleOutputComponent {

  // todo add the possibility for realtime updates

  @Input() set outputId(value: string | undefined) {
    if (!value) {
      this.consoleOutput = '';
      return;
    }

    this.outputFilesService.fetchOutput(value).subscribe(output => {
      this.consoleOutput = output;
    });
  }

  consoleOutput: string = "";

  constructor(private outputFilesService: OutputFilesService) {
  }
}
