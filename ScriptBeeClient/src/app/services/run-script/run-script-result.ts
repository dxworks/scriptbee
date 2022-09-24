export interface RunScriptResult {
  runId: string;
  runIndex: number;
  projectId: string;
  results: OutputResult[];
}

export interface OutputResult {
  outputId: string;
  outputType: string;
  path: string;
}

