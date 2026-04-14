import * as vscode from 'vscode';
import { storage } from './utils/storage';
import { registerCommands } from './commands/index';
import { initStatusBar } from './ui/statusBar';
import { initTreeView } from './views/ScriptBeeTreeView';
import { logger } from './utils/logger';
import { hideMetaFiles } from './utils/workspaceUtils';
import { RemoteScriptProvider } from './providers/RemoteScriptProvider';

export function activate(context: vscode.ExtensionContext) {
  logger.init();
  logger.log('ScriptBee VS Code Extension activated.');
  storage.setContext(context);
  hideMetaFiles();

  context.subscriptions.push(vscode.workspace.registerTextDocumentContentProvider(RemoteScriptProvider.scheme, new RemoteScriptProvider()));

  registerCommands(context);
  initStatusBar(context);
  initTreeView(context);
}

export function deactivate() {}
