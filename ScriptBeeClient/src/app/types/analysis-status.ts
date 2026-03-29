export type AnalysisStatusType = 'Started' | 'Running' | 'Finished' | 'Cancelled';

export interface AnalysisStatus {
  instanceId: string;
  status: AnalysisStatusType;
  scriptId: string;
  creationDate: Date;
  finishedDate?: Date;
  errors?: AnalysisError[];
}

export interface AnalysisError {
  message: string;
}
