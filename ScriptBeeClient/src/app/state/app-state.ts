import { OutputState } from "./outputs/output.state";
import { ProjectDetailsState } from "./project-details/project-details.state";
import { LoadersState } from "./loaders/loaders.state";
import { LinkersState } from "./linkers/linkers.state";

export interface AppState {
  projectDetails: ProjectDetailsState;
  loaders: LoadersState;
  linkers: LinkersState;
  outputState: OutputState;
}
