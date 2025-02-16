import { TreeNode } from '../types/tree-node';

export function updateTreeNodeArray(treeNodes: TreeNode[], name: string, children: string[]) {
  const indexOfNode = treeNodes.findIndex((node) => node.name === name);
  if (indexOfNode === -1) {
    treeNodes.push({
      name: name,
      children: children.map((file) => ({
        name: file,
      })),
    });
  } else {
    treeNodes[indexOfNode] = {
      name: treeNodes[indexOfNode].name,
      children: children.map((file) => ({
        name: file,
      })),
    };
  }
  return treeNodes;
}
