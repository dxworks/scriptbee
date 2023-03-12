import { TreeNode } from '../../shared/tree-node';

export interface ProjectData {
  projectId: string;
  projectName: string;
  creationDate: string;
  linker?: string;
  loaders: string[];
  savedFiles: TreeNode[];
  loadedFiles: TreeNode[];
}
