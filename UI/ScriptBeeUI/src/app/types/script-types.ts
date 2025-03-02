export type ParameterType = 'string' | 'integer' | 'float' | 'boolean';

export type ParameterValue = string | number | boolean;

export interface Parameter {
  name: string;
  type: ParameterType;
  value?: ParameterValue;
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
