import { createReducer, on } from "@ngrx/store";
import { OutputState } from "./output.state";
import { setOutput } from "./output.actions";

export const initialState: OutputState = {
  results: []
}

export const outputReducer = createReducer(
  initialState,
  on(setOutput, (state, {runIndex, results, scriptName}) => {
    return {...state, runIndex, results, scriptName}
  }),
);
