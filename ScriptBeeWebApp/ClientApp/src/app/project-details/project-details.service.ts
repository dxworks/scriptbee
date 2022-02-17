import {Injectable} from '@angular/core';
import {Project} from "../projects/project";
import {BehaviorSubject} from "rxjs";
import {TreeNode} from "../shared/tree/tree.component";

@Injectable({
  providedIn: 'root'
})
export class ProjectDetailsService {

  project: BehaviorSubject<Project> = new BehaviorSubject<Project>(undefined);
  loaders: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  context: BehaviorSubject<TreeNode[]> = new BehaviorSubject<TreeNode[]>([]);

  clearData() {
    this.project.next(undefined);
    this.loaders.next([]);
    this.context.next([]);
  }
}
