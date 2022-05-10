import {Injectable} from '@angular/core';
import {Project} from '../projects/project';
import {BehaviorSubject} from 'rxjs';
import {TreeNode} from '../shared/tree-node';
import {RunScriptResult} from '../services/run-script/run-script-result';

@Injectable({
  providedIn: 'root'
})
export class ProjectDetailsService {

  project: BehaviorSubject<Project> = new BehaviorSubject<Project>(undefined);
  loaders: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  linkers: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  context: BehaviorSubject<TreeNode[]> = new BehaviorSubject<TreeNode[]>([]);
  lastRunResult: BehaviorSubject<RunScriptResult> = new BehaviorSubject<RunScriptResult>(undefined);
  lastRunErrorMessage: BehaviorSubject<string> = new BehaviorSubject<string>('');
  lastOperationFailure: BehaviorSubject<string> = new BehaviorSubject<string>('');

  clearData() {
    this.project.next(undefined);
    this.loaders.next([]);
    this.linkers.next([]);
    this.context.next([]);
    this.lastRunResult.next(undefined);
    this.lastRunErrorMessage.next('');
    this.lastOperationFailure.next('');
  }
}
