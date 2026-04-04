export type CalculationInstanceStatus = 'Allocating' | 'Running' | 'Deallocating' | 'NotFound' | 'Unknown';

export interface InstanceInfo {
  id: string;
  creationDate: string;
  status: CalculationInstanceStatus;
}
