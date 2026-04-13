import axiosInstance from './axiosInstance';

export interface ProjectResponse {
  id: string;
  name: string;
}

export interface ProjectListResponse {
  data: ProjectResponse[];
}

export async function getAllProjects(baseUrl: string): Promise<ProjectResponse[]> {
  const response = await axiosInstance.get<ProjectListResponse>('/api/projects', {
    baseURL: baseUrl,
  });

  return response.data?.data || [];
}
