import { TreeNode } from '../../../../shared/tree-node';

export interface FileTreeNode extends TreeNode {
  filePath: string;
  srcPath: string;
  children?: FileTreeNode[];
}
