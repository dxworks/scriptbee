export interface InstanceInfo {
  id: string;
  creationDate: string;
  // TODO FIXIT(#29, #70): remove this properties and make a separate api call to get the context
  loaders: string[];
  linkers: string[];
  loadedModels: Record<string, string[]>;
}
