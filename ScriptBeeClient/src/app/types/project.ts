export interface Project {
  id: string;
  name: string;
  creationDate: string;
}

export interface ProjectListResponse {
  projects: Project[];
}

export interface CreateProjectRequest {
  id: string;
  name: string;
}

export interface CreateProjectResponse {
  id: string;
  name: string;
  creationDate: string;
}

export interface ProjectStructureNode {
  id: string;
  name: string;
  path: string;
  absolutePath: string;
  children?: ProjectStructureNode[];
}
