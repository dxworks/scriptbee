import * as vscode from 'vscode';
import { connectionService } from '../services/connectionService';
import { COMMAND_SELECT_PROJECT } from '../commands/commandIds';

let statusBarItem: vscode.StatusBarItem;

export function initStatusBar(context: vscode.ExtensionContext) {
  statusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Left, 100);
  statusBarItem.command = COMMAND_SELECT_PROJECT;
  context.subscriptions.push(statusBarItem);
  updateStatusBar();
  statusBarItem.show();
}

export async function updateStatusBar() {
  if (!statusBarItem) {
    return;
  }
  const activeConnection = await connectionService.getActiveConnection();

  if (!activeConnection) {
    statusBarItem.text = '$(debug-disconnect) ScriptBee: No Connection';
    statusBarItem.tooltip = 'Click to set ScriptBee connection';
  } else {
    const projectId = activeConnection.projectId;
    if (!projectId) {
      statusBarItem.text = `$(link) ScriptBee: ${activeConnection.name} (No Project)`;
      statusBarItem.tooltip = `Connected to ${activeConnection.url}. Click to select a project.`;
    } else {
      statusBarItem.text = `$(project) ScriptBee: ${activeConnection.name} / ${projectId}`;
      statusBarItem.tooltip = `Connected to ${activeConnection.url}, Project: ${projectId}. Click to change project.`;
    }
  }
}
