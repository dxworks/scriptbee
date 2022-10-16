import { createSelector } from "@ngrx/store";
import { AppState } from "../app-state";

export const selectOutputState = (state: AppState) => state.outputState;

export const selectLastRunOutput = createSelector(selectOutputState, state => state);
