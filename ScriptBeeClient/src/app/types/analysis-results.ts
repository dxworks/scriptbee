export interface AnalysisConsoleOutput {
  content: string;
}

export interface AnalysisRunErrorOutput {
  data: AnalysisRunError[];
}

export interface AnalysisFileOutput {
  data: AnalysisFile[];
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
