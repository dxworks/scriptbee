import axiosInstance from './axiosInstance';

export interface WebProjectFileNode {
  id: string;
  name: string;
  path: string;
  absolutePath: string;
  type: 'file' | 'folder';
  hasChildren: boolean;
}

export interface WebGetProjectFilesResponse {
  data: WebProjectFileNode[];
  totalCount: number;
  offset: number;
  limit: number;
}

export interface WebScriptData {
  id: string;
  name: string;
  path: string;
  absolutePath: string;
  language: string;
}

export async function getProjectFiles(
  baseUrl: string,
  projectId: string,
  parentId?: string,
  offset: number = 0,
  limit: number = 50
): Promise<WebGetProjectFilesResponse> {
  const response = await axiosInstance.get<WebGetProjectFilesResponse>(`/api/projects/${projectId}/files`, {
    baseURL: baseUrl,
    params: {
      parentId,
      offset,
      limit,
    },
  });

  return response.data;
}

export async function createScript(baseUrl: string, projectId: string, path: string, language: string): Promise<WebScriptData> {
  const response = await axiosInstance.post<WebScriptData>(
    `/api/projects/${projectId}/scripts`,
    {
      path,
      language,
    },
    {
      baseURL: baseUrl,
    }
  );
  return response.data;
}

export async function updateScriptContent(baseUrl: string, projectId: string, scriptId: string, content: string): Promise<void> {
  await axiosInstance.put(`/api/projects/${projectId}/scripts/${scriptId}/content`, content, {
    baseURL: baseUrl,
    headers: {
      'Content-Type': 'text/plain',
    },
  });
}

export async function getScriptContent(baseUrl: string, projectId: string, scriptId: string): Promise<string> {
  const response = await axiosInstance.get<string>(`/api/projects/${projectId}/scripts/${scriptId}/content`, {
    baseURL: baseUrl,
    responseType: 'text',
  });
  return response.data;
}
