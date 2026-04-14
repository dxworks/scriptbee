import * as vscode from 'vscode';
import * as CommandIds from './commandIds';
import { addConnection } from './connection/addConnection';
import { editConnection } from './connection/editConnection';
import { switchConnection } from './connection/switchConnection';
import { deleteConnection } from './connection/deleteConnection';
import { selectProject } from './project/selectProject';
import { selectInstance } from './project/selectInstance';
import { generateClasses } from './project/generateClasses';
import { pullScripts } from './project/pullScripts';
import { pushScripts } from './project/pushScripts';
import { pushScript } from './project/pushScript';
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
  const selectInstanceCmd = vscode.commands.registerCommand(CommandIds.COMMAND_SELECT_INSTANCE, selectInstance);
  const generateClassesCmd = vscode.commands.registerCommand(CommandIds.COMMAND_GENERATE_CLASSES, generateClasses);
  const pullScriptsCmd = vscode.commands.registerCommand(CommandIds.COMMAND_PULL_SCRIPTS, pullScripts);
  const pushScriptsCmd = vscode.commands.registerCommand(CommandIds.COMMAND_PUSH_SCRIPTS, pushScripts);
  const pushScriptCmd = vscode.commands.registerCommand(CommandIds.COMMAND_PUSH_SCRIPT, pushScript);
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
    selectInstanceCmd,
    generateClassesCmd,
    pullScriptsCmd,
    pushScriptsCmd,
    pushScriptCmd,
    syncScriptsCmd,
    openProjectFolderCmd,
    compareWithRemoteCmd,
    refreshCmd
  );
}
