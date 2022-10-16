import { Injectable } from '@angular/core';
import { BehaviorSubject } from "rxjs";
import { ConsoleOutputComponent } from "../../project-details/output/console-output/console-output.component";
import { OutputErrorsComponent } from "../../project-details/output/output-errors/output-errors.component";

export interface Result {
  name: string;
  component?: any;
  module?: any
}

@Injectable({
  providedIn: 'root'
})
// todo to be updated when ui plugins is ready
export class ResultsService {
  resultsComponents: BehaviorSubject<Result[]> = new BehaviorSubject<Result[]>([
    {
      name: 'Output Errors',
      component: OutputErrorsComponent
    },
    {
      name: 'Console Output',
      component: ConsoleOutputComponent
    },
    // {
    //   name: 'File Output',
    //   component: FileOutputComponent
    // }
  ]);

  addData(data: Result) {
    this.resultsComponents.next([...this.resultsComponents.getValue(), data]);
  }
}
