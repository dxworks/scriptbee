export interface TreeNodeWithParent {
  name: string;
  children?: TreeNodeWithParent[];
  parent?: TreeNodeWithParent;
}

export interface TreeNode {
  // TODO: pass node data
  name: string;
  children?: TreeNode[];
}
