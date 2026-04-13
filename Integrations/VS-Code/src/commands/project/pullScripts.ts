import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';

export async function pullScripts(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection) {
    vscode.window.showErrorMessage('No active connection found for pull');
    return;
  }

  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Pulling scripts for project ${connection.projectId}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.pull(connection.id);
        vscode.window.showInformationMessage(`Successfully pulled scripts for project ${connection.projectId}`);
      } catch (error: any) {
        vscode.window.showErrorMessage(`Failed to pull scripts: ${error.message}`);
      }
    }
  );
}
