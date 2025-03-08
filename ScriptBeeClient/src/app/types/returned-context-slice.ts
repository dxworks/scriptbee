export interface ReturnedContextSlice {
  name: string;
  models: string[];
}

export type ProjectContext = ProjectContextSlice[];

export interface ProjectContextSlice {
  model: string;
  pluginIds: string[];
}
