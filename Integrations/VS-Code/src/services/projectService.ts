import * as vscode from 'vscode';
import * as fs from 'fs/promises';
import { getAllProjects, ProjectResponse } from '../api/projects';
import { connectionService } from './connectionService';
import { getProjectSrcPath } from '../utils/workspaceUtils';

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

  public async openProjectFolder(projectId: string, inWorkspace: boolean = false): Promise<void> {
    const srcPath = getProjectSrcPath(projectId);
    await fs.mkdir(srcPath, { recursive: true });

    const uri = vscode.Uri.file(srcPath);

    if (inWorkspace) {
      const folderCount = vscode.workspace.workspaceFolders?.length || 0;
      vscode.workspace.updateWorkspaceFolders(folderCount, 0, { uri, name: `ScriptBee: ${projectId}` });
    } else {
      await vscode.commands.executeCommand('vscode.openFolder', uri, { forceNewWindow: true });
    }
  }
}

export const projectService = new ProjectService();
