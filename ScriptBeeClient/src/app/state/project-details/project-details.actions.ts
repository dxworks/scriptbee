import { createAction, props } from "@ngrx/store";
import { ProjectContext, ProjectData } from "./project";

export const setSavedFiles = createAction(
  '[ScriptBee] Set Saved Files',
  props<{ loader: string, files: string[] }>()
)

export const setLoadedModels = createAction(
  '[ScriptBee] Set Loaded Models',
  props<{ loader: string, files: string[] }>()
);

export const clearContext = createAction(
  '[ScriptBee] Clear Context'
);

export const fetchProject = createAction(
  '[ScriptBee] Fetch Project',
  props<{ projectId: string }>()
);

export const fetchProjectSuccess = createAction(
  '[ScriptBee] Fetch Project Success',
  props<{ data: ProjectData, context: ProjectContext }>()
);

export const fetchProjectFailure = createAction(
  '[ScriptBee] Fetch Project Failure',
  props<{ error: string }>()
);
