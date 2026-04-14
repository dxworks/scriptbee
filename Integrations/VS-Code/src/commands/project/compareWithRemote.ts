import * as vscode from 'vscode';
import { storage } from '../../utils/storage';
import { connectionService } from '../../services/connectionService';
import { RemoteScriptProvider } from '../../providers/RemoteScriptProvider';
import { logger } from '../../utils/logger';
import { showErrorWithCopy } from '../../utils/errorUtils';

export async function compareWithRemote(uri?: vscode.Uri) {
  const fileUri = uri || vscode.window.activeTextEditor?.document.uri;

  if (!fileUri) {
    vscode.window.showErrorMessage('No file selected for comparison');
    return;
  }

  try {
    const meta = await storage.getScriptMeta(fileUri);
    if (!meta) {
      vscode.window.showWarningMessage('This file is not tracked by ScriptBee. Pull or push it first.');
      return;
    }

    const connection = await connectionService.getActiveConnection();
    if (!connection || !connection.projectId) {
      vscode.window.showErrorMessage('No active ScriptBee connection or project selected.');
      return;
    }

    const remoteUri = vscode.Uri.parse(
      `${RemoteScriptProvider.scheme}://remote/${connection.projectId}/${meta.id}?baseUrl=${encodeURIComponent(connection.url)}`
    );

    const fileName = fileUri.path.split('/').pop() || 'file';
    logger.log(`Comparing local file ${fileName} with remote ID ${meta.id}`);

    await vscode.commands.executeCommand('vscode.diff', fileUri, remoteUri, `${fileName} (Local <-> Remote)`);
  } catch (error) {
    logger.error('Failed to open diff view', error);
    showErrorWithCopy('Failed to open diff view', error).catch(console.error);
  }
}
