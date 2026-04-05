export type InstanceStatus = 'Allocating' | 'Running' | 'Deallocating' | 'NotFound';

export interface InstanceInfo {
  id: string;
  creationDate: string;
  status: InstanceStatus;
}
