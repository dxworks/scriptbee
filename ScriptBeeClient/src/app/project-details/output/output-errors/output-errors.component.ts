import { Component, Input } from '@angular/core';
import { OutputFilesService } from "../../../services/output/output-files.service";

@Component({
  selector: 'app-output-errors',
  templateUrl: './output-errors.component.html',
  styleUrls: ['./output-errors.component.scss']
})
export class OutputErrorsComponent {

  @Input() set outputId(value: string | undefined) {
    if (!value) {
      this.errors = '';
      return;
    }

    this.outputFilesService.fetchOutput(value).subscribe(output => {
      this.errors = output;
    });
  }

  errors: string = "";

  constructor(private outputFilesService: OutputFilesService) {
  }
}
