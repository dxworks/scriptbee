import { createReducer, on } from "@ngrx/store";
import { EntityAdapter, createEntityAdapter } from "@ngrx/entity";
import { OutputData, OutputState } from "./output.state";
import {
  clearOutput,
  fetchOutputData,
  fetchOutputDataFailure,
  fetchOutputDataSuccess,
  setOutput
} from "./output.actions";

export function selectOutputDataPath(outputData: OutputData) {
  return outputData.path;
}

export const outputAdapter: EntityAdapter<OutputData> = createEntityAdapter<OutputData>({
  selectId: selectOutputDataPath
});

export const initialState: OutputState = outputAdapter.getInitialState();

export const outputReducer = createReducer(
  initialState,
  on(setOutput, (state, outputData) => {
    return outputAdapter.setOne(outputData, state);
  }),
  on(clearOutput, (state, {outputId}) => {
    return outputAdapter.removeOne(outputId, state);
  }),
  on(fetchOutputData, (state, {outputId}) => {
    return outputAdapter.updateOne({id: outputId, changes: {loading: true}}, state);
  }),
  on(fetchOutputDataSuccess, (state, {outputId, output}) => {
    return outputAdapter.updateOne({
      id: outputId,
      changes: {loading: false, data: output, loadingError: undefined}
    }, state);
  }),
  on(fetchOutputDataFailure, (state, {outputId, error}) => {
    return outputAdapter.updateOne({
      id: outputId,
      changes: {loading: false, data: undefined, loadingError: error}
    }, state);
  })
);
