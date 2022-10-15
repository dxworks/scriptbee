import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state";
import { catchError, of, switchMap } from "rxjs";
import { map } from "rxjs/operators";
import { LoaderService } from "../../services/loader/loader.service";
import { fetchLinkers, fetchLinkersFailure, fetchLinkersSuccess } from "./linkers.actions";

@Injectable()
export class LinkersEffects {
  constructor(private actions$: Actions, private store: Store<AppState>, private loaderService: LoaderService) {
  }

  fetchLinkers = createEffect(() =>
    this.actions$.pipe(
      ofType(fetchLinkers),
      switchMap(() =>
        this.loaderService.getAllLoaders().pipe(
          map((linkers) => fetchLinkersSuccess({linkers: linkers})),
          catchError((error) => of(fetchLinkersFailure({error: error})))
        ))
    ));
}
