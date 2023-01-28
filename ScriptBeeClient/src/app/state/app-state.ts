import { OutputState } from "./outputs/output.state";
import { ProjectDetailsState } from "./project-details/project-details.state";
import { ScriptTreeState } from "./script-tree/script-tree.state";

export interface AppState {
  projectDetails: ProjectDetailsState;
  scriptTree: ScriptTreeState;
  outputState: OutputState;
}
