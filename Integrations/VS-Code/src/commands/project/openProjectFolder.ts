import * as vscode from 'vscode';
import { projectService } from '../../services/projectService';
import { ScriptBeeTreeItem } from '../../views/ScriptBeeTreeView';
import { logger } from '../../utils/logger';
import { showErrorWithCopy } from '../../utils/errorUtils';

export async function openProjectFolder(item?: ScriptBeeTreeItem) {
  const connection = item?.connection;
  if (!connection || !connection.projectId) {
    vscode.window.showErrorMessage('No active project found to open');
    return;
  }

  const option = await vscode.window.showQuickPick(
    [
      { label: 'Open in Current Window', detail: 'Replaces the current folder', value: false },
      { label: 'Add to Workspace', detail: 'Adds to the current workspace folders', value: true },
    ],
    { placeHolder: 'How would you like to open the project folder?' }
  );

  if (option) {
    try {
      await projectService.openProjectFolder(connection.projectId, option.value);
      logger.log(`Successfully opened project folder ${connection.projectId} in workspace ${option.value}`);
    } catch (error: any) {
      logger.error(`Failed to open project folder ${connection.projectId}`, error);
      await showErrorWithCopy('Failed to open project folder', error);
    }
  }
}
