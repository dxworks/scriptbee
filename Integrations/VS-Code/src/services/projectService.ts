import { getAllProjects, ProjectResponse } from '../api/projects';
import { connectionService } from './connectionService';

export class ProjectService {
  public async fetchProjects(): Promise<ProjectResponse[]> {
    return await getAllProjects();
  }

  public async setSelectedProject(projectId: string | undefined): Promise<void> {
    const activeConnection = await connectionService.getActiveConnection();
    if (activeConnection) {
      activeConnection.projectId = projectId;
      await connectionService.updateConnection(activeConnection);
    }
  }

  public async getSelectedProjectId(): Promise<string | undefined> {
    const activeConnection = await connectionService.getActiveConnection();
    return activeConnection?.projectId;
  }
}

export const projectService = new ProjectService();
