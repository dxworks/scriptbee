import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state";

import { catchError, of, switchMap } from "rxjs";
import { map } from "rxjs/operators";
import { LoaderService } from "../../services/loader/loader.service";
import { fetchLoaders, fetchLoadersFailure, fetchLoadersSuccess } from "./loaders.actions";

@Injectable()
export class LoadersEffects {
  constructor(private actions$: Actions, private store: Store<AppState>, private loaderService: LoaderService) {
  }

  fetchLoaders = createEffect(() =>
    this.actions$.pipe(
      ofType(fetchLoaders),
      switchMap(() =>
        this.loaderService.getAllLoaders().pipe(
          map((loaders) => fetchLoadersSuccess({loaders: loaders})),
          catchError((error) => of(fetchLoadersFailure({error: error})))
        ))
    ));
}
