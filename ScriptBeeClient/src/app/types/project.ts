import { ScriptLanguage, ScriptParameter } from './script-types';

export interface ProjectFile {
  id: string;
  name: string;
}

export interface Project {
  id: string;
  name: string;
  creationDate: string;
  savedFiles: Record<string, ProjectFile[]>;
  loadedFiles: Record<string, ProjectFile[]>;
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

export interface ProjectScript {
  id: string;
  name: string;
  path: string;
  absolutePath: string;
  scriptLanguage: ScriptLanguage;
  parameters: ScriptParameter[];
}

export interface ProjectFileNode {
  id: string;
  name: string;
  path: string;
  absolutePath: string;
  type: string;
  hasChildren: boolean;
}

export interface GetProjectFilesResponse {
  data: ProjectFileNode[];
  totalCount: number;
  offset: number;
  limit: number;
}
