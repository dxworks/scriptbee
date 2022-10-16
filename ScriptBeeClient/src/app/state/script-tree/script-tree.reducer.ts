import { createReducer, on } from "@ngrx/store";
import { ScriptTreeNode, ScriptTreeState } from "./script-tree.state";
import {
  createScript,
  createScriptFailure,
  createScriptSuccess,
  fetchScriptTree,
  fetchScriptTreeFailure,
  fetchScriptTreeSuccess,
  scriptTreeLeafClick
} from "./script-tree.actions";
import { FileTreeNode } from "../../project-details/scripts-content/fileTreeNode";

// todo stop assuming that src is the root

const initialState: ScriptTreeState = {
  tree: []
};

export const scriptTreeReducer = createReducer(
  initialState,
  on(fetchScriptTree, (state, {projectId}) => {
    return {...state, projectId, loading: true}
  }),
  on(fetchScriptTreeSuccess, (state, {tree}) => {
    return {...state, tree: convertTreeToScriptTree(tree, 0), loading: false}
  }),
  on(fetchScriptTreeFailure, (state, {error}) => {
    return {...state, fetchError: error, loading: false}
  }),
  on(scriptTreeLeafClick, (state, {node}) => {
    return {...state, clickedLeaf: node}
  }),
  on(createScript, (state, {projectId, scriptType, scriptPath}) => {
    return {...state, scriptCreation: {projectId, scriptType, scriptPath}, createScriptLoading: true}
  }),
  on(createScriptSuccess, (state, {node}) => {
    console.log(state.tree);
    const tree = insertNodeInTree(state.tree, node);
    console.log(tree)
    return {...state, tree: tree, createScriptLoading: false}
  }),
  on(createScriptFailure, (state, {error}) => {
    return {...state, createScriptError: error, createScriptLoading: false}
  })
);

function convertTreeToScriptTree(tree: FileTreeNode[], level: number): ScriptTreeNode[] {
  return tree.map(node => {
    return {
      id: node.srcPath,
      name: node.name,
      children: node.children ? convertTreeToScriptTree(node.children, level + 1) : undefined,
      level: level,
      filePath: node.filePath,
      isExpanded: false
    }
  });
}

// todo insert in alphabetical order
// todo finish implementation
function insertNodeInTree(tree: ScriptTreeNode[], node: FileTreeNode): ScriptTreeNode[] {
  if (!node.srcPath.includes("/")) {
    return [{
      ...tree[0],
      children: [...tree[0].children, {
        id: node.srcPath,
        name: node.name,
        children: undefined,
        level: 0,
        filePath: node.filePath,
        isExpanded: false
      }]
    }];
  }

  return tree;
}
