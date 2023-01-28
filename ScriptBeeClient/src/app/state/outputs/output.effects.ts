import { Actions } from "@ngrx/effects";
import { AppState } from "../app-state";
import { Store } from "@ngrx/store";
import { Injectable } from "@angular/core";

@Injectable()
export class OutputEffects {
  constructor(private actions$: Actions, private store: Store<AppState>) {
  }


}
