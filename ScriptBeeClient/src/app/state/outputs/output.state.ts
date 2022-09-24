import { EntityState } from "@ngrx/entity";

export interface OutputData {
  outputId: string;
  projectId: string;
  outputType: string;
  path: string;
  loading: boolean;
  data?: any;
  loadingError?: string;
}

export interface OutputState extends EntityState<OutputData> {
}
