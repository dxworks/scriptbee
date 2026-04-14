import * as vscode from 'vscode';
import * as CommandIds from './commandIds';
import { addConnection } from './connection/addConnection';
import { editConnection } from './connection/editConnection';
import { switchConnection } from './connection/switchConnection';
import { deleteConnection } from './connection/deleteConnection';
import { selectProject } from './project/selectProject';
import { pullScripts } from './project/pullScripts';
import { pushScripts } from './project/pushScripts';
import { syncScripts } from './project/syncScripts';
import { openProjectFolder } from './project/openProjectFolder';
import { compareWithRemote } from './project/compareWithRemote';
import { refreshUI } from './ui/refreshUI';

export function registerCommands(context: vscode.ExtensionContext) {
  const addConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_ADD_CONNECTION, addConnection);
  const editConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_EDIT_CONNECTION, editConnection);
  const switchConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_SWITCH_CONNECTION, switchConnection);
  const deleteConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_DELETE_CONNECTION, deleteConnection);
  const selectProjectCmd = vscode.commands.registerCommand(CommandIds.COMMAND_SELECT_PROJECT, selectProject);
  const pullScriptsCmd = vscode.commands.registerCommand(CommandIds.COMMAND_PULL_SCRIPTS, pullScripts);
  const pushScriptsCmd = vscode.commands.registerCommand(CommandIds.COMMAND_PUSH_SCRIPTS, pushScripts);
  const syncScriptsCmd = vscode.commands.registerCommand(CommandIds.COMMAND_SYNC_SCRIPTS, syncScripts);
  const openProjectFolderCmd = vscode.commands.registerCommand(CommandIds.COMMAND_OPEN_PROJECT_FOLDER, openProjectFolder);
  const compareWithRemoteCmd = vscode.commands.registerCommand(CommandIds.COMMAND_COMPARE_WITH_REMOTE, compareWithRemote);
  const refreshCmd = vscode.commands.registerCommand(CommandIds.COMMAND_REFRESH_UI, refreshUI);

  context.subscriptions.push(
    addConnectionCmd,
    editConnectionCmd,
    switchConnectionCmd,
    deleteConnectionCmd,
    selectProjectCmd,
    pullScriptsCmd,
    pushScriptsCmd,
    syncScriptsCmd,
    openProjectFolderCmd,
    compareWithRemoteCmd,
    refreshCmd
  );
}
