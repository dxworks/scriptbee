export interface ScriptTreeState {
  projectId?: string;
  tree: ScriptTreeNode[];
  loading?: boolean
  fetchError?: string;
  clickedLeaf?: ScriptTreeNode;

  createScriptLoading?: boolean;
  createScriptError?: string;
}

export interface ScriptTreeNode {
  id: string;
  name: string;
  filePath: string;
  children?: ScriptTreeNode[];
  level: number;
  isExpanded: boolean;
}
