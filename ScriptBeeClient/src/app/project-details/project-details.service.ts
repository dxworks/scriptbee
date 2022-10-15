import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { RunScriptResult } from '../services/run-script/run-script-result';

@Injectable({
  providedIn: 'root'
})
export class ProjectDetailsService {

  // todo use the output state
  lastRunResult = new BehaviorSubject<RunScriptResult | undefined>(undefined);
  lastRunErrorMessage = new BehaviorSubject<string>('');

  clearData() {
    this.lastRunResult.next(undefined);
    this.lastRunErrorMessage.next('');
  }
}
