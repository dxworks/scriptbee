import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';

export async function syncScripts(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection) {
    vscode.window.showErrorMessage('No active connection found for sync');
    return;
  }

  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Syncing scripts for project ${connection.projectId}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.sync(connection.id);
        vscode.window.showInformationMessage(`Successfully synced scripts for project ${connection.projectId}`);
      } catch (error: any) {
        vscode.window.showErrorMessage(`Failed to sync scripts: ${error.message}`);
      }
    }
  );
}
