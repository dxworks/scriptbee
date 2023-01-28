import { createReducer, on } from "@ngrx/store";
import { ProjectDetailsState } from "./project-details.state";
import {
  clearContext,
  fetchProject,
  fetchProjectFailure,
  fetchProjectSuccess,
  setLoadedModels,
  setSavedFiles
} from "./project-details.actions";
import { TreeNode } from "../../shared/tree-node";

export const initialState: ProjectDetailsState = {
  projectDetailsId: "",
}

function updateTreeNodeArray(savedFiles: TreeNode[], name: string, children: string[]) {
  const indexOfNode = savedFiles.findIndex(node => node.name === name);
  if (indexOfNode === -1) {
    savedFiles.push({
      name: name,
      children: children.map(file => ({
        name: file
      }))
    });
  } else {
    savedFiles[indexOfNode] = {
      name: savedFiles[indexOfNode].name,
      children: children.map(file => ({
        name: file
      }))
    };
  }
  return savedFiles;
}


export const projectDetailsReducer = createReducer(
  initialState,
  on(setSavedFiles, (state, {loader, files}) => {
    const prevSavedFiles = [...state.project.data.savedFiles];

    const savedFiles = updateTreeNodeArray(prevSavedFiles, loader, files);

    return {
      ...state,
      project: {
        ...state.project,
        data: {
          ...state.project.data,
          savedFiles
        }
      }
    }
  }),
  on(setLoadedModels, (state, {loader, files}) => {
    const oldContext = [...state.project.context];

    const context = updateTreeNodeArray(oldContext, loader, files);

    return {
      ...state,
      project: {
        ...state.project,
        context
      }
    }
  }),
  on(clearContext, (state) => {
    return {
      ...state,
      project: {
        ...state.project,
        context: []
      }
    }
  }),
  on(fetchProject, (state, {projectId}) => {
    return {...state, projectId, loadingProject: true, fetchProjectError: undefined};
  }),
  on(fetchProjectSuccess, (state, {data, context}) => {
    return {
      ...state,
      project: {...state.project, data: data, context: context},
      loadingProject: false,
      fetchProjectError: undefined
    };
  }),
  on(fetchProjectFailure, (state, {error}) => {
    return {...state, loadingProject: false, fetchProjectError: error};
  }),
);
