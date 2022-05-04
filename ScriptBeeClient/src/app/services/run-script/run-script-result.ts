import {OutputFile} from '../output/output-file';

export interface RunScriptResult {
  runId: string;
  projectId: string;
  consoleOutputName: string;
  outputFiles: OutputFile[];
  errors?: string;
}

