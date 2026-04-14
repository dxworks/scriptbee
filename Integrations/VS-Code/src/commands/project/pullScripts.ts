import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { logger } from '../../utils/logger';

export async function pullScripts(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection) {
    vscode.window.showErrorMessage('No active connection found for pull');
    return;
  }

  logger.log(`Starting scripts pull loop for project ID: ${connection.projectId} / connection ID: ${connection.id}`);
  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Pulling scripts for project ${connection.projectId}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.pull(connection.id);
        logger.log(`Successfully completed pull for project ${connection.projectId}`);
        vscode.window.setStatusBarMessage(`$(check) Successfully pulled scripts for project ${connection.projectId}`, 5000);
      } catch (error: any) {
        logger.error(`Failed to pull scripts for project ${connection.projectId}`, error);
        showErrorWithCopy('Failed to pull scripts', error).catch(logger.error);
      }
    }
  );
}
