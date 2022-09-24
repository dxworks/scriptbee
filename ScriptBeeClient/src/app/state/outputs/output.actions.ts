import { createAction, props } from "@ngrx/store";
import { OutputData } from "./output.state";

export const setOutput = createAction(
  '[ScriptBee] Set Output',
  props<OutputData>()
);

export const clearOutput = createAction(
  '[ScriptBee] Clear Output',
  props<{ outputId: string; }>()
);

export const fetchOutputData = createAction(
  '[ScriptBee] Fetch Output Data',
  props<{ outputId: string }>()
);

export const fetchOutputDataSuccess = createAction(
  '[ScriptBee] Fetch Output Data Success',
  props<{ outputId: string, output: any }>()
);

export const fetchOutputDataFailure = createAction(
  '[ScriptBee] Fetch Output Data Failure',
  props<{ outputId: string, error: string }>()
);
