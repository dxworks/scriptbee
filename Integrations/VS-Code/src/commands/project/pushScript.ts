import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { logger } from '../../utils/logger';
import * as path from 'path';

export async function pushScript(uri?: vscode.Uri) {
  const fileUri = uri || vscode.window.activeTextEditor?.document.uri;

  if (!fileUri) {
    vscode.window.showErrorMessage('No file selected for push');
    return;
  }

  const fileName = path.basename(fileUri.fsPath);
  logger.log(`Starting push for script: ${fileName}`);

  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Pushing script ${fileName}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.pushFileByUri(fileUri);
        logger.log(`Successfully completed push for script ${fileName}`);
        vscode.window.setStatusBarMessage(`$(check) Successfully pushed script ${fileName}`, 5000);
      } catch (error: any) {
        logger.error(`Failed to push script ${fileName}`, error);
        showErrorWithCopy(`Failed to push script ${fileName}`, error).catch(console.error);
      }
    }
  );
}
