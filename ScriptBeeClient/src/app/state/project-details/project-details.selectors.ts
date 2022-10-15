import { createSelector } from "@ngrx/store";
import { AppState } from "../app-state";

export const projectDetailsState = (state: AppState) => state.projectDetails;

export const selectProjectDetailsLoading = createSelector(projectDetailsState, (state) => ({
    loading: state.loadingProject,
    error: state.fetchProjectError
  })
);

export const selectProjectDetails = createSelector(projectDetailsState, state => state.project);

