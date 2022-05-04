import {TreeNode} from "../shared/tree-node";

export interface Project {
  projectId: string;
  projectName: string;
  creationDate: string;
  linker?: string;
  loaders: string[];
  savedFiles: TreeNode[];
  loadedFiles: TreeNode[];
}
