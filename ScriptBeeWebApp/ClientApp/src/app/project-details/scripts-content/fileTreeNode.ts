import {TreeNode} from '../../shared/tree/tree.component';

export interface FileTreeNode extends TreeNode {
  filePath: string;
  srcPath: string;
}
