import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';

export async function pushScripts(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection) {
    vscode.window.showErrorMessage('No active connection found for push');
    return;
  }

  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Pushing scripts for project ${connection.projectId}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.push(connection.id);
        vscode.window.showInformationMessage(`Successfully pushed scripts for project ${connection.projectId}`);
      } catch (error: any) {
        vscode.window.showErrorMessage(`Failed to push scripts: ${error.message}`);
      }
    }
  );
}
