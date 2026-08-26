import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { authService } from '../../services/authService';
import { COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';
import { Connection } from '../../utils/storage';

export async function logout(connectionArg?: CommandConnectionArg): Promise<void> {
  const connection = await resolveConnection(connectionArg);
  if (!connection) {
    return;
  }

  await authService.logout(connection.id);
  vscode.window.showInformationMessage(`Logged out from ${connection.name}.`);
  await vscode.commands.executeCommand(COMMAND_REFRESH_UI);
}

async function resolveConnection(connectionArg?: CommandConnectionArg): Promise<Connection | undefined> {
  const connectionId = getConnectionId(connectionArg);
  const connections = await connectionService.getConnections();

  if (connections.length === 0) {
    vscode.window.showErrorMessage('No connections saved.');
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
    placeHolder: 'Select ScriptBee connection to logout',
  });

  return selected?.connection;
}
