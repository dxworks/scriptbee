import { createReducer, on } from "@ngrx/store";
import { LoadersState } from "./loaders.state";
import { fetchLoaders, fetchLoadersFailure, fetchLoadersSuccess } from "./loaders.actions";

export const initialState: LoadersState = {};

export const loadersReducer = createReducer(
  initialState,
  on(fetchLoaders, (state, {projectId}) => {
    return {...state, projectId, loadingLoaders: true, loadingLoadersError: undefined};
  }),
  on(fetchLoadersSuccess, (state, {loaders}) => {
    return {...state, loaders: loaders, loadingLoaders: false, loadingLoadersError: undefined};
  }),
  on(fetchLoadersFailure, (state, {error}) => {
    return {...state, loadingLoaders: false, loadingLoadersError: error};
  }),
);
