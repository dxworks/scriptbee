export type ParameterType = 'string' | 'integer' | 'float' | 'boolean';

export type ParameterValue = string | number | boolean;

export interface ScriptParameter {
  name: string;
  type: ParameterType;
  value?: ParameterValue;
}

export interface ScriptLanguage {
  name: string;
  extension: string;
}

export interface CreateScriptRequest {
  path: string;
  language: string;
  parameters: ScriptParameter[];
}

export interface UpdateScriptRequest {
  language: string;
  parameters: ScriptParameter[];
}
