import { createReducer, on } from "@ngrx/store";
import { ProjectDetailsState } from "./project-details.state";
import { fetchProject, fetchProjectFailure, fetchProjectSuccess, setSavedFiles } from "./project-details.actions";

export const initialState: ProjectDetailsState = {
  projectDetailsId: "",
}

export const projectDetailsReducer = createReducer(
  initialState,
  on(setSavedFiles, (state, {loader, files}) => {
    const savedFiles = [...state.project.data.savedFiles];

    const indexOfNode = savedFiles.findIndex(node => node.name === loader);
    if (indexOfNode === -1) {
      savedFiles.push({
        name: loader,
        children: files.map(file => ({
          name: file
        }))
      });
    } else {
      savedFiles[indexOfNode] = {
        name: savedFiles[indexOfNode].name,
        children: files.map(file => ({
          name: file
        }))
      };
    }

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
  on(fetchProject, (state, {projectId}) => {
    return {...state, projectId, loadingProject: true, loadingProjectError: undefined};
  }),
  on(fetchProjectSuccess, (state, {data, context}) => {
    return {
      ...state,
      project: {...state.project, data: data, context: context},
      loadingProject: false,
      loadingProjectError: undefined
    };
  }),
  on(fetchProjectFailure, (state, {error}) => {
    return {...state, loadingProject: false, loadingProjectError: error};
  }),
);
