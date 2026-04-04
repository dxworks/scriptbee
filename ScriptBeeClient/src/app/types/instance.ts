export type CalculationInstanceStatus = 'Allocating' | 'Running' | 'Deallocating' | 'NotFound';

export interface InstanceInfo {
  id: string;
  creationDate: string;
  status: CalculationInstanceStatus;
}
