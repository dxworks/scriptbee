import axiosInstance from './axiosInstance';

export interface ProjectResponse {
  id: string;
  name: string;
}

export interface ProjectListResponse {
  data: ProjectResponse[];
}

export async function getAllProjects(): Promise<ProjectResponse[]> {
  const response = await axiosInstance.get<ProjectListResponse>('/api/projects');

  return response.data?.data || [];
}
