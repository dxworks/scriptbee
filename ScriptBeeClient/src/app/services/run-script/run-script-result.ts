export interface RunScriptResult {
  index: number;
  linker: string;
  loadedFiles: Map<string, string[]>;
  scriptName: string;
  results: OutputResult[];
}

export interface OutputResult {
  id: string;
  name: string;
  type: string;
}

