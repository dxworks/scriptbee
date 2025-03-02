export interface InstanceInfo {
  id: string;
  loaders: string[];
  linkers: string[];
  loadedModels: Record<string, string[]>;
  creationDate: string;
}
