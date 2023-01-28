import { createAction, props } from "@ngrx/store";
import { Result } from "./output.state";

export const setOutput = createAction(
  '[ScriptBee] Set Output',
  props<{ runIndex: number, scriptName: string, results: Result[] }>()
);

