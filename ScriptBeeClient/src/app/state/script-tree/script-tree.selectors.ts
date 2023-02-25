import { AppState } from '../app-state';
import { createSelector } from '@ngrx/store';

export const scriptTreeState = (state: AppState) => state.scriptTree;

export const selectScriptTreeLeafClick = createSelector(scriptTreeState, (state) => state.clickedLeaf);
