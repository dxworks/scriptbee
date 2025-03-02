export type ParameterType = 'string' | 'integer' | 'float' | 'boolean';

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
