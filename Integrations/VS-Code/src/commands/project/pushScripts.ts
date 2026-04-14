import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { logger } from '../../utils/logger';

export async function pushScripts(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection) {
    vscode.window.showErrorMessage('No active connection found for push');
    return;
  }

  logger.log(`Starting scripts push loop for project ID: ${connection.projectId} / connection ID: ${connection.id}`);
  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Pushing scripts for project ${connection.projectId}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.push(connection.id);
        logger.log(`Successfully completed push for project ${connection.projectId}`);
        vscode.window.setStatusBarMessage(`$(check) Successfully pushed scripts for project ${connection.projectId}`, 5000);
      } catch (error: any) {
        logger.error(`Failed to push scripts for project ${connection.projectId}`, error);
        showErrorWithCopy('Failed to push scripts', error).catch(console.error);
      }
    }
  );
}
