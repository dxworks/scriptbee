import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { authService } from '../../services/authService';
import { COMMAND_ADD_CONNECTION, COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';
import { Connection } from '../../utils/storage';

export async function login(connectionArg?: CommandConnectionArg): Promise<void> {
  const connection = await resolveConnection(connectionArg);
  if (!connection) {
    return;
  }

  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `ScriptBee: Logging in to ${connection.name}...`,
      cancellable: false,
    },
    async () => {
      try {
        const authConfig = await authService.getAuthConfig(connection.url);
        if (authConfig?.authMode?.toLowerCase() === 'development') {
          vscode.window.showInformationMessage(`Authentication is not required for ${connection.name} (Development mode).`);
          return;
        }

        await authService.login(connection);
        vscode.window.showInformationMessage(`Successfully authenticated with ${connection.name}.`);
        await vscode.commands.executeCommand(COMMAND_REFRESH_UI);
      } catch (error) {
        const message = error instanceof Error ? error.message : String(error);
        vscode.window.showErrorMessage(`Login failed for ${connection.name}: ${message}`);
      }
    }
  );
}

async function resolveConnection(connectionArg?: CommandConnectionArg): Promise<Connection | undefined> {
  const connectionId = getConnectionId(connectionArg);
  const connections = await connectionService.getConnections();

  if (connections.length === 0) {
    const action = 'Add Connection';
    const result = await vscode.window.showErrorMessage('No connections saved.', action);
    if (result === action) {
      await vscode.commands.executeCommand(COMMAND_ADD_CONNECTION);
    }
    return undefined;
  }

  if (connectionId) {
    return connections.find((c) => c.id === connectionId);
  }

  const activeConnection = await connectionService.getActiveConnection();
  if (activeConnection) {
    return activeConnection;
  }

  const items = connections.map((c) => ({
    label: c.name,
    description: c.url,
    connection: c,
  }));

  const selected = await vscode.window.showQuickPick(items, {
    placeHolder: 'Select ScriptBee connection to login',
  });

  return selected?.connection;
}
