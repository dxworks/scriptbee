import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { COMMAND_ADD_CONNECTION, COMMAND_ON_PROJECT_SELECTED, COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';

export async function switchConnection(connectionArg: CommandConnectionArg) {
  const connectionId = getConnectionId(connectionArg);

  if (connectionId) {
    await connectionService.setActiveConnection(connectionId);
    vscode.commands.executeCommand(COMMAND_REFRESH_UI);
    vscode.commands.executeCommand(COMMAND_ON_PROJECT_SELECTED);
    return;
  }

  const connections = await connectionService.getConnections();
  const activeId = connectionService.getActiveConnectionId();

  if (connections.length === 0) {
    const action = 'Add Connection';
    const result = await vscode.window.showErrorMessage('No connections saved.', action);
    if (result === action) {
      vscode.commands.executeCommand(COMMAND_ADD_CONNECTION);
    }
    return;
  }

  const items = connections.map((c) => ({
    label: c.name,
    description: `${c.url}${c.id === activeId ? ' (Active)' : ''}`,
    connectionId: c.id,
  }));

  const selected = await vscode.window.showQuickPick(items, {
    placeHolder: 'Select active ScriptBee connection',
  });

  if (selected) {
    await connectionService.setActiveConnection(selected.connectionId);
    vscode.window.setStatusBarMessage(`Switched to connection: ${selected.label}`, 3000);
    vscode.commands.executeCommand(COMMAND_REFRESH_UI);
    vscode.commands.executeCommand(COMMAND_ON_PROJECT_SELECTED);
  }
}
