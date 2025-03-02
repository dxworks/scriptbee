export interface AnalysisConsoleOutput {
  content: string;
}

export interface AnalysisRunErrorOutput {
  errors: AnalysisRunError[];
}

export interface AnalysisFileOutput {
  files: AnalysisFile[];
}

export interface AnalysisRunError {
  title: string;
  message: string;
  severity: string;
}

export interface AnalysisFile {
  id: string;
  name: string;
  type: string;
}
