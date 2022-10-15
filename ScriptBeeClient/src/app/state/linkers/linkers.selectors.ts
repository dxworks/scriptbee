import { AppState } from "../app-state";
import { createSelector } from "@ngrx/store";

export const linkersState = (state: AppState) => state.linkers;

export const selectLinkers = createSelector(linkersState, (state) => state.linkers);

export const selectLinkersFetchError = createSelector(linkersState, (state) => state.fetchError);
