import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { COMMAND_REFRESH_UI } from '../commandIds';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';

export async function deleteConnection(connectionArg: CommandConnectionArg) {
  const connections = await connectionService.getConnections();
  let idToDelete = getConnectionId(connectionArg);

  if (!idToDelete) {
    const items = connections.map((c) => ({
      label: c.name,
      description: c.url,
      id: c.id,
    }));

    const selected = await vscode.window.showQuickPick(items, {
      placeHolder: 'Select a connection to delete',
    });

    if (selected) {
      idToDelete = selected.id;
    }
  }

  if (idToDelete) {
    await connectionService.deleteConnection(idToDelete);
    vscode.window.setStatusBarMessage('Connection deleted.', 3000);
    vscode.commands.executeCommand(COMMAND_REFRESH_UI);
  }
}
