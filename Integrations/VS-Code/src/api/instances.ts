import axiosInstance from './axiosInstance';

export interface InstanceResponse {
  id: string;
  creationDate: string;
  status: string;
}

export interface WebGetProjectInstancesListResponse {
  data: InstanceResponse[];
}

export async function getProjectInstances(baseUrl: string, projectId: string): Promise<InstanceResponse[]> {
  const response = await axiosInstance.get<WebGetProjectInstancesListResponse>(`/api/projects/${projectId}/instances`, {
    baseURL: baseUrl,
  });

  return response.data?.data || [];
}
