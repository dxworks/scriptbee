import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { projectService } from '../../services/projectService';
import { Connection } from '../../utils/storage';
import { COMMAND_ADD_CONNECTION, COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';

export async function selectProject(connectionArg?: CommandConnectionArg) {
  let connection: Connection | undefined;
  const connectionId = getConnectionId(connectionArg);

  if (connectionId) {
    const connections = await connectionService.getConnections();
    connection = connections.find((c) => c.id === connectionId);
  } else {
    connection = await connectionService.getActiveConnection();
  }

  if (!connection) {
    const action = 'Add Connection';
    const result = await vscode.window.showErrorMessage('ScriptBee connection is not set.', action);
    if (result === action) {
      vscode.commands.executeCommand(COMMAND_ADD_CONNECTION);
    }
    return;
  }

  try {
    const projects = await vscode.window.withProgress(
      {
        location: vscode.ProgressLocation.Notification,
        title: 'ScriptBee',
        cancellable: false,
      },
      async (progress) => {
        progress.report({ message: `Fetching projects for ${connection?.name}...` });
        return await projectService.fetchProjects();
      }
    );

    const currentProjectId = connection.projectId;
    const items: (vscode.QuickPickItem & { projectId?: string })[] = [];

    items.push({
      label: '$(circle-slash) Clear Project Selection',
      description: 'Disassociate current project from the connection',
      projectId: undefined,
    });

    if (projects && projects.length > 0) {
      items.push(
        ...projects.map((p) => ({
          label: p.name,
          description: p.id === currentProjectId ? `${p.id} (Currently Selected)` : p.id,
          projectId: p.id,
          alwaysShow: true,
        }))
      );
    }

    const selected = await vscode.window.showQuickPick(items, {
      placeHolder: `Select a project for ${connection.name}`,
    });

    if (selected !== undefined) {
      connection.projectId = selected.projectId;
      await connectionService.updateConnection(connection);

      const message = selected.projectId ? `Selected project: ${selected.label}` : 'Project disassociated.';
      vscode.window.setStatusBarMessage(message, 3000);
      vscode.commands.executeCommand(COMMAND_REFRESH_UI);
    }
  } catch (e) {
    const message = e instanceof Error ? e.message : String(e);
    vscode.window.showErrorMessage(`Failed to fetch projects: ${message}`);
  }
}
