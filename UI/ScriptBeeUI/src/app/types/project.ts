export interface Project {
  id: string;
  name: string;
  creationDate: string;
}

export interface ProjectListResponse {
  projects: Project[];
}
