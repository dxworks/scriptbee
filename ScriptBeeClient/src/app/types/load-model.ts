export interface Loader {
  id: string;
  name: string;
}

export interface LoadModel {
  projectId: string;
  nodes: LoadModelNode[];
}

interface LoadModelNode {
  loaderName: string;
  models: string[];
}
