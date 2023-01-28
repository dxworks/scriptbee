import { AppState } from "../app-state";
import { createSelector } from "@ngrx/store";

export const loadersState = (state: AppState) => state.loaders;

export const selectLoaders = createSelector(loadersState, (state) => state.loaders);

export const selectLoadersFetchError = createSelector(loadersState, (state) => state.fetchError);
