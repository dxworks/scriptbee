import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectOutput } from "../../../state/outputs/output.selectors";
import { filter, map } from "rxjs/operators";
import { fetchOutputData } from "../../../state/outputs/output.actions";

@Component({
  selector: 'app-console-output',
  templateUrl: './console-output.component.html',
  styleUrls: ['./console-output.component.scss']
})
export class ConsoleOutputComponent implements OnInit {
  private consoleOutputStream = this.store.select(selectOutput('Console'));

  consoleOutput: string = "";

  constructor(private store: Store) {
  }

  ngOnInit(): void {
    this.consoleOutputStream.pipe(
      filter(output => output !== undefined),
      filter(output => output.length > 0),
      map(output => output[0]),
      filter(output => !output?.loading),
    ).subscribe(consoleOutput => {
      if (!consoleOutput) {
        return;
      }

      if (consoleOutput.data) {
        this.consoleOutput = consoleOutput.data;
      } else if (consoleOutput.loadingError) {
        // todo handle the error better (show a message to the user)
        console.error(consoleOutput.loadingError);
      } else {
        this.store.dispatch(fetchOutputData({outputId: consoleOutput.outputId}));
      }
    });
  }
}
