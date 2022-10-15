import { createReducer, on } from "@ngrx/store";
import { LinkersState } from "./linkers.state";
import { fetchLinkers, fetchLinkersFailure, fetchLinkersSuccess } from "./linkers.actions";

export const initialState: LinkersState = {
  linkers: [],
}

export const linkersReducer = createReducer(
  initialState,
  on(fetchLinkers, (state, {projectId}) => {
    return {...state, projectId, loadingLoaders: true, loadingLoadersError: undefined};
  }),
  on(fetchLinkersSuccess, (state, {linkers}) => {
    return {...state, linkers: linkers, loadingLoaders: false, loadingLoadersError: undefined};
  }),
  on(fetchLinkersFailure, (state, {error}) => {
    return {...state, loadingLoaders: false, loadingLoadersError: error};
  }),
);
