import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state";
import { ProjectService } from "../../services/project/project.service";
import { fetchProject, fetchProjectFailure, fetchProjectSuccess } from "./project-details.actions";
import { catchError, forkJoin, of, switchMap } from "rxjs";
import { map } from "rxjs/operators";

@Injectable()
export class ProjectDetailsEffects {
  constructor(private actions$: Actions, private store: Store<AppState>, private projectService: ProjectService) {
  }

  fetchProject = createEffect(() =>
    this.actions$.pipe(
      ofType(fetchProject),
      switchMap(action =>
        forkJoin([this.projectService.getProject(action.projectId), this.projectService.getProjectContext(action.projectId)]).pipe(
          map(([projectData, context]) => fetchProjectSuccess({data: projectData, context: context})),
          catchError((error) => of(fetchProjectFailure({error: error})))
        ))
    ));
}
