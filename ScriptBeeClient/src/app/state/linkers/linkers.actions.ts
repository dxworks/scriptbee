import { createAction, props } from "@ngrx/store";

export const fetchLinkers = createAction(
  '[ScriptBee] Fetch Linkers',
  props<{ projectId: string }>()
);

export const fetchLinkersSuccess = createAction(
  '[ScriptBee] Fetch Linkers Success',
  props<{ linkers: string[] }>()
);

export const fetchLinkersFailure = createAction(
  '[ScriptBee] Fetch Linkers Failure',
  props<{ error: string }>()
);
