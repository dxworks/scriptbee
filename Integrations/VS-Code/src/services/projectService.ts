import { getAllProjects, ProjectResponse } from '../api/projects';
import { connectionService } from './connectionService';

export class ProjectService {
  public async fetchProjects(baseUrl: string): Promise<ProjectResponse[]> {
    return await getAllProjects(baseUrl);
  }

  public async setSelectedProject(connectionId: string, projectId: string | undefined): Promise<void> {
    const connections = await connectionService.getConnections();
    const connection = connections.find((c) => c.id === connectionId);

    if (connection) {
      connection.projectId = projectId;
      await connectionService.updateConnection(connection);
    }
  }

  public async getSelectedProjectId(connectionId: string): Promise<string | undefined> {
    const connections = await connectionService.getConnections();
    const connection = connections.find((c) => c.id === connectionId);
    return connection?.projectId;
  }
}

export const projectService = new ProjectService();
