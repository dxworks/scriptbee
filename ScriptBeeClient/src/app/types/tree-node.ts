export interface TreeNodeWithParent<T> {
  name: string;
  children?: TreeNodeWithParent<T>[];
  parent?: TreeNodeWithParent<T>;
  data: T;
}

export interface TreeNode<T> {
  name: string;
  children?: TreeNode<T>[];
  data: T;
}

export interface TreeAction<T> {
  label: string;
  icon: string;
  callback: (node: TreeNode<T>) => void;
}
