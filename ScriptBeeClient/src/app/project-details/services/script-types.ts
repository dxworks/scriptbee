// TODO: remove
export enum ScriptTypes {
  python = 'python',
  javascript = 'javascript',
  csharp = 'csharp',
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

export interface CreateScriptDataParameter {
  name: string;
  type: ParameterType;
  value: string;
}

export interface CreateScriptData {
  projectId: string;
  filePath: string;
  scriptType: string;
  parameters: CreateScriptDataParameter[];
}
