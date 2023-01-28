import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state";
import { catchError, of, switchMap } from "rxjs";
import { map } from "rxjs/operators";
import {
  createScript,
  createScriptFailure,
  createScriptSuccess,
  fetchScriptTree,
  fetchScriptTreeFailure,
  fetchScriptTreeSuccess
} from "./script-tree.actions";
import { FileSystemService } from "../../services/file-system/file-system.service";

@Injectable()
export class ScriptTreeEffects {
  constructor(private actions$: Actions, private store: Store<AppState>,
              private fileSystemService: FileSystemService) {
  }

  fetchScriptTree = createEffect(() =>
    this.actions$.pipe(
      ofType(fetchScriptTree),
      switchMap(action =>
        this.fileSystemService.getFileSystem(action.projectId).pipe(
          map((response) => fetchScriptTreeSuccess({tree: [response]})),
          catchError((error) => of(fetchScriptTreeFailure({error: error.error})))
        ))
    ));

  createScript = createEffect(() =>
    this.actions$.pipe(
      ofType(createScript),
      switchMap(action =>
        this.fileSystemService.createScript(action.projectId, action.scriptPath, action.scriptType).pipe(
          map((response) => createScriptSuccess({node: response})),
          catchError((error) => of(createScriptFailure({error: error.error})))
        ))
    ));
}
