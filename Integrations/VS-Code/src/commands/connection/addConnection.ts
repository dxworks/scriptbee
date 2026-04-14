import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { COMMAND_REFRESH_UI } from '../commandIds';
import { showErrorWithCopy } from '../../utils/errorUtils';
import { logger } from '../../utils/logger';

export async function addConnection() {
  const name = await vscode.window.showInputBox({
    prompt: 'Enter a name for this connection',
    placeHolder: 'e.g. Local, Production',
    validateInput: (value) => (value ? null : 'Name is required'),
  });

  if (!name) {
    return;
  }

  const url = await vscode.window.showInputBox({
    prompt: 'Enter ScriptBee URL',
    value: 'http://localhost:5000',
    validateInput: (value) => {
      try {
        new URL(value);
        return null;
      } catch (e) {
        return 'Invalid URL';
      }
    },
  });

  if (!url) {
    return;
  }

  try {
    await connectionService.addConnection(name, url);
    vscode.window.setStatusBarMessage(`Connection "${name}" added.`, 3000);
    vscode.commands.executeCommand(COMMAND_REFRESH_UI);
  } catch (error) {
    logger.error(`Failed to add connection "${name}"`, error);
    await showErrorWithCopy('Failed to add connection', error);
  }
}
