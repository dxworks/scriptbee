export interface TreeNodeWithParent<T> {
  children?: TreeNodeWithParent<T>[];
  parent?: TreeNodeWithParent<T>;
  data: T;
}

export interface TreeNode<T> {
  children?: TreeNode<T>[];
  data: T;
}

export type TreeActionType = 'file' | 'folder' | 'all';

export interface TreeAction<T> {
  label: string;
  icon: string;
  type: TreeActionType;
  callback: (node: TreeNode<T>) => void;
}
