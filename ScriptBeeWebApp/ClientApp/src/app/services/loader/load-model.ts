export interface LoadModel {
  projectId: string;
  nodes: LoadModelNode[];
}

interface LoadModelNode {
  loaderName: string;
  models: string[];
}
