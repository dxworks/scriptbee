import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { Connection } from '../../utils/storage';
import { COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';

export async function editConnection(connectionArg: CommandConnectionArg) {
  const connections = await connectionService.getConnections();
  let connection: Connection | undefined;

  const connectionId = getConnectionId(connectionArg);

  if (connectionId) {
    connection = connections.find((c) => c.id === connectionId);
  } else if (connections.length > 0) {
    const items = connections.map((c) => ({
      label: c.name,
      description: c.url,
      connection: c,
    }));

    const selected = await vscode.window.showQuickPick(items, {
      placeHolder: 'Select a connection to edit',
    });

    if (selected) {
      connection = selected.connection;
    }
  }

  if (!connection) {
    if (connections.length === 0) {
      vscode.window.showInformationMessage('No connections available to edit.');
    }
    return;
  }

  const newName = await vscode.window.showInputBox({
    prompt: 'Edit connection name',
    value: connection.name,
    validateInput: (value) => (value ? null : 'Name is required'),
  });

  if (newName === undefined) {
    return;
  }

  const newUrl = await vscode.window.showInputBox({
    prompt: 'Edit ScriptBee URL',
    value: connection.url,
    validateInput: (value) => {
      try {
        new URL(value);
        return null;
      } catch (e) {
        return 'Invalid URL';
      }
    },
  });

  if (newUrl === undefined) {
    return;
  }

  try {
    connection.name = newName;
    connection.url = newUrl;
    await connectionService.updateConnection(connection);
    vscode.window.setStatusBarMessage(`Connection "${newName}" updated.`, 3000);
    vscode.commands.executeCommand(COMMAND_REFRESH_UI);
  } catch (e) {
    const message = e instanceof Error ? e.message : String(e);
    vscode.window.showErrorMessage(`Failed to update connection: ${message}`);
  }
}
