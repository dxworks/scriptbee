import * as vscode from 'vscode';
import { storage } from './utils/storage';
import { registerCommands } from './commands';
import { initStatusBar } from './ui/statusBar';
import { initTreeView } from './views/ScriptBeeTreeView';
import { logger } from './utils/logger';
import { hideMetaFiles } from './utils/workspaceUtils';
import { RemoteScriptProvider } from './providers/RemoteScriptProvider';
import { liveUpdatesService } from './services/liveUpdatesService';
import { autoPushService } from './services/autoPushService';
import { COMMAND_ON_PROJECT_SELECTED } from './commands/commandIds';

export function activate(context: vscode.ExtensionContext) {
  logger.init();
  logger.log('ScriptBee VS Code Extension activated.');
  storage.setContext(context);
  void hideMetaFiles();

  context.subscriptions.push(vscode.workspace.registerTextDocumentContentProvider(RemoteScriptProvider.scheme, new RemoteScriptProvider()));

  registerCommands(context);
  initStatusBar(context);
  initTreeView(context);

  void liveUpdatesService.start();
  autoPushService.start();

  context.subscriptions.push(
    vscode.workspace.onDidChangeConfiguration((e) => {
      if (e.affectsConfiguration('scriptbee.enableLiveUpdates')) {
        void liveUpdatesService.start();
      }
      if (e.affectsConfiguration('scriptbee.enableAutoPush')) {
        autoPushService.start();
      }
    }),
    vscode.commands.registerCommand(COMMAND_ON_PROJECT_SELECTED, () => {
      void liveUpdatesService.start();
    })
  );
}

export function deactivate() {
  void liveUpdatesService.stop();
  autoPushService.stop();
}
