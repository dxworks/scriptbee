import { Project } from "./project";

export interface ProjectDetailsState {
  project?: Project;
  projectDetailsId: string;
  loadingProject?: boolean
  fetchProjectError?: string;
}
