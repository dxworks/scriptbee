import { Actions, createEffect, ofType } from "@ngrx/effects";
import { AppState } from "../app-state";
import { Store } from "@ngrx/store";
import { OutputFilesService } from "../../services/output/output-files.service";
import { Injectable } from "@angular/core";
import { fetchOutputData, fetchOutputDataFailure, fetchOutputDataSuccess } from "./output.actions";
import { catchError, of, switchMap } from "rxjs";
import { map } from "rxjs/operators";

@Injectable()
export class OutputEffects {
  constructor(private actions$: Actions, private store: Store<AppState>, private outputService: OutputFilesService) {
  }

  fetchOutput = createEffect(() =>
    this.actions$.pipe(
      ofType(fetchOutputData),
      switchMap((action) =>
        this.outputService.fetchOutput(action.outputId).pipe(
          map((output) => fetchOutputDataSuccess({outputId: action.outputId, output: output})),
          catchError((error) => of(fetchOutputDataFailure({outputId: action.outputId, error: error})))
        ))
    ))
}
