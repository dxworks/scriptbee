import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { logger } from '../../utils/logger';

export async function syncScripts(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection) {
    vscode.window.showErrorMessage('No active connection found for sync');
    return;
  }

  logger.log(`Starting scripts sync loop for project ID: ${connection.projectId} / connection ID: ${connection.id}`);
  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Syncing scripts for project ${connection.projectId}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.sync(connection.id);
        logger.log(`Successfully completed sync for project ${connection.projectId}`);
        vscode.window.setStatusBarMessage(`$(check) Successfully synced scripts for project ${connection.projectId}`, 5000);
      } catch (error: any) {
        logger.error(`Failed to sync scripts for project ${connection.projectId}`, error);
        showErrorWithCopy('Failed to sync scripts', error).catch(logger.error);
      }
    }
  );
}
