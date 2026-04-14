import * as vscode from 'vscode';
import { scriptSyncService } from '../../services/scriptSyncService';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { logger } from '../../utils/logger';
import * as path from 'path';

export async function pullScript(uri?: vscode.Uri) {
  const fileUri = uri || vscode.window.activeTextEditor?.document.uri;

  if (!fileUri) {
    vscode.window.showErrorMessage('No file selected for pull');
    return;
  }

  const fileName = path.basename(fileUri.fsPath);
  logger.log(`Starting pull for script: ${fileName}`);

  await vscode.window.withProgress(
    {
      location: vscode.ProgressLocation.Notification,
      title: `Pulling script ${fileName}...`,
      cancellable: false,
    },
    async () => {
      try {
        await scriptSyncService.pullFileByUri(fileUri);
        logger.log(`Successfully completed pull for script ${fileName}`);
        vscode.window.setStatusBarMessage(`$(check) Successfully pulled script ${fileName}`, 5000);
      } catch (error: any) {
        logger.error(`Failed to pull script ${fileName}`, error);
        showErrorWithCopy(`Failed to pull script ${fileName}`, error).catch(console.error);
      }
    }
  );
}
