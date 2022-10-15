import { createSelector } from "@ngrx/store";
import { AppState } from "../app-state";
import { OutputState } from "./output.state";

export const selectOutputState = (state: AppState) => state.outputState;

export const selectOutput = (outputType: string) => createSelector(
  selectOutputState,
  (state: OutputState) => {
    return Object.entries(state.entities).map(([, value]) => value).filter((output) => output?.outputType === outputType);
  },
);
