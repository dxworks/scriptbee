// TODO: remove
export enum ScriptTypes {
  javascript = 'javascript',
}

export enum ParameterType {
  string = 'string',
  integer = 'integer',
  float = 'float',
  boolean = 'boolean',
}

export const ParameterTypeValues = Object.values(ParameterType);

export interface Parameter {
  id: string;
  name: string;
  type: ParameterType;
  value?: string;
  nameError?: string;
}

export interface ScriptLanguage {
  name: string;
  extension: string;
}

export interface ScriptDataParameter {
  name: string;
  type: ParameterType;
  value: string;
}

export interface CreateScriptData {
  projectId: string;
  filePath: string;
  scriptLanguage: string;
  parameters: ScriptDataParameter[];
}

export interface UpdateScriptData {
  id: string;
  projectId: string;
  parameters: ScriptDataParameter[];
}

export interface ScriptData {
  id: string;
  projectId: string;
  name: string;
  filePath: string;
  absolutePath: string;
  scriptLanguage: string;
  parameters: ScriptDataParameter[];
}

export type CreateScriptResponse = ScriptData;

export type UpdateScriptResponse = ScriptData;

export interface ScriptFileStructureNode {
  name: string;
  path: string;
  absolutePath: string;
  isDirectory: boolean;
  scriptData?: ScriptData;
  level: number;
  children?: ScriptFileStructureNode[];
}
