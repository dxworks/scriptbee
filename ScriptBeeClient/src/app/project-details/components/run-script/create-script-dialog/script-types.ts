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
}
