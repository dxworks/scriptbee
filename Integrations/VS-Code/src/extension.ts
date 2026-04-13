import * as vscode from 'vscode';
import { storage } from './utils/storage';
import { registerCommands } from './commands/index';
import { initStatusBar } from './ui/statusBar';
import { initTreeView } from './views/ScriptBeeTreeView';

export function activate(context: vscode.ExtensionContext) {
  storage.setContext(context);

  registerCommands(context);
  initStatusBar(context);
  initTreeView(context);
}

export function deactivate() {}
