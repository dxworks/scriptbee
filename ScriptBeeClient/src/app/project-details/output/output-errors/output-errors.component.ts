import { Component, OnInit } from '@angular/core';
import { selectOutput } from "../../../state/outputs/output.selectors";
import { OutputFilesService } from "../../../services/output/output-files.service";
import { filter } from "rxjs/operators";
import { switchMap } from "rxjs";
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-output-errors',
  templateUrl: './output-errors.component.html',
  styleUrls: ['./output-errors.component.scss']
})
export class OutputErrorsComponent implements OnInit {
 private buildErrorsStream = this.store.select(selectOutput('BuildError'));

  buildErrors: string;

  constructor(private store: Store, private outputFilesService: OutputFilesService) {
  }

  ngOnInit(): void {
    this.buildErrorsStream.pipe(
      filter(output => output !== undefined),
      filter(output => output.length > 0),
      switchMap(buildError => this.outputFilesService.getConsoleOutputContent(buildError[0].path))
    ).subscribe(buildErrors => {
      this.buildErrors = buildErrors;
    });
  }
}
