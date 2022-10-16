import { OutputState } from "./outputs/output.state";
import { ProjectDetailsState } from "./project-details/project-details.state";
import { LoadersState } from "./loaders/loaders.state";
import { LinkersState } from "./linkers/linkers.state";
import { ScriptTreeState } from "./script-tree/script-tree.state";

export interface AppState {
  projectDetails: ProjectDetailsState;
  scriptTree: ScriptTreeState;
  loaders: LoadersState;
  linkers: LinkersState;
  outputState: OutputState;
}
