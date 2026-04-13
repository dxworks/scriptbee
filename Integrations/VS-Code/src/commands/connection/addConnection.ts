import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { COMMAND_REFRESH_UI } from '../commandIds';

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
  } catch (e) {
    const message = e instanceof Error ? e.message : String(e);
    vscode.window.showErrorMessage(`Failed to add connection: ${message}`);
  }
}
