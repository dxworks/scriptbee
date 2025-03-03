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
