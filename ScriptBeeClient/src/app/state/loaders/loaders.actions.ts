import { createAction, props } from "@ngrx/store";

export const fetchLoaders = createAction(
  '[ScriptBee] Fetch Loaders',
  props<{ projectId: string }>()
);

export const fetchLoadersSuccess = createAction(
  '[ScriptBee] Fetch Loaders Success',
  props<{ loaders: string[] }>()
);

export const fetchLoadersFailure = createAction(
  '[ScriptBee] Fetch Loaders Failure',
  props<{ error: string }>()
);
