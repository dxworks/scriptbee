import { Parameter } from '../../../services/script-types';

export interface EditParametersDialogData {
  scriptId: string;
  projectId: string;
  parameters: Parameter[];
}
