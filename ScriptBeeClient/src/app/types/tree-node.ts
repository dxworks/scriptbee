export interface TreeNodeWithParent {
  name: string;
  children?: TreeNodeWithParent[];
  parent?: TreeNodeWithParent;
}

export interface TreeNode {
  name: string;
  children?: TreeNode[];
}
