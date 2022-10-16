import { AppState } from "../app-state";
import { createSelector } from "@ngrx/store";

export const scriptTreeState = (state: AppState) => state.scriptTree;

export const selectScriptTreeLoading = createSelector(scriptTreeState, (state) => ({
    loading: state.loading,
    error: state.fetchError
  })
);

export const selectScriptTree = createSelector(scriptTreeState, state => state.tree);

export const selectScriptTreeLeafClick = createSelector(scriptTreeState, state => state.clickedLeaf);

export const selectScriptCreationLoading = createSelector(scriptTreeState, state => {
  return {
    loading: state.createScriptLoading,
    error: state.createScriptError
  };
});

