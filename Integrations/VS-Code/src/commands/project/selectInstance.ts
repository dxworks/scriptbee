import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { projectService } from '../../services/projectService';
import { Connection } from '../../utils/storage';
import { COMMAND_ADD_CONNECTION, COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';
import { logger } from '../../utils/logger';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { getProjectInstances } from '../../api/instances';

export async function selectInstance(connectionArg?: CommandConnectionArg) {
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

  if (!connection.projectId) {
    vscode.window.showErrorMessage('Please select a project first.');
    return;
  }

  try {
    const instances = await vscode.window.withProgress(
      {
        location: vscode.ProgressLocation.Notification,
        title: 'ScriptBee',
        cancellable: false,
      },
      async (progress) => {
        progress.report({ message: `Fetching instances for ${connection?.projectId}...` });
        return await getProjectInstances(connection!.url, connection!.projectId!);
      }
    );

    const currentInstanceId = connection.instanceId;
    const items: (vscode.QuickPickItem & { instanceId?: string })[] = [];

    items.push({
      label: '$(circle-slash) Clear Instance Selection',
      description: 'Disassociate current instance from the connection',
      instanceId: undefined,
    });

    if (instances && instances.length > 0) {
      items.push(
        ...instances.map((i) => ({
          label: i.id,
          description: i.id === currentInstanceId ? `${i.status} (Currently Selected)` : i.status,
          instanceId: i.id,
          alwaysShow: true,
        }))
      );
    }

    const selected = await vscode.window.showQuickPick(items, {
      placeHolder: `Select an instance for ${connection.projectId}`,
    });

    if (selected !== undefined) {
      await projectService.setSelectedInstance(connection.id, selected.instanceId);
      const message = selected.instanceId ? `Selected instance: ${selected.label}` : 'Instance disassociated.';
      vscode.window.setStatusBarMessage(message, 3000);
      vscode.commands.executeCommand(COMMAND_REFRESH_UI);
    }
  } catch (error) {
    logger.error(`Failed to fetch instances for ${connection.projectId}`, error);
    await showErrorWithCopy(`Failed to fetch instances for ${connection.projectId}`, error);
  }
}
