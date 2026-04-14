import * as vscode from 'vscode';
import { COMMAND_REFRESH_TREE_VIEW } from '../commandIds';
import { updateStatusBar } from '../../ui/statusBar';

export function refreshUI() {
  void vscode.commands.executeCommand(COMMAND_REFRESH_TREE_VIEW);
  void updateStatusBar();
}
