export type AnalysisStatusType = 'Started' | 'Running' | 'Finished' | 'Cancelled';

export interface AnalysisStatus {
  id: string;
  instanceId: string;
  status: AnalysisStatusType;
  scriptId: string;
  scriptFileId?: string;
  creationDate: Date;
  finishedDate?: Date;
  errors?: AnalysisError[];
}

export interface AnalysisError {
  message: string;
}
