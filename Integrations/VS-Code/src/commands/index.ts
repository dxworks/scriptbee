import * as vscode from 'vscode';
import * as CommandIds from './commandIds';
import { addConnection } from './connection/addConnection';
import { editConnection } from './connection/editConnection';
import { switchConnection } from './connection/switchConnection';
import { deleteConnection } from './connection/deleteConnection';
import { selectProject } from './project/selectProject';
import { refreshUI } from './ui/refreshUI';

export function registerCommands(context: vscode.ExtensionContext) {
  const addConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_ADD_CONNECTION, addConnection);
  const editConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_EDIT_CONNECTION, editConnection);
  const switchConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_SWITCH_CONNECTION, switchConnection);
  const deleteConnectionCmd = vscode.commands.registerCommand(CommandIds.COMMAND_DELETE_CONNECTION, deleteConnection);
  const selectProjectCmd = vscode.commands.registerCommand(CommandIds.COMMAND_SELECT_PROJECT, selectProject);
  const refreshCmd = vscode.commands.registerCommand(CommandIds.COMMAND_REFRESH_UI, refreshUI);

  context.subscriptions.push(addConnectionCmd, editConnectionCmd, switchConnectionCmd, deleteConnectionCmd, selectProjectCmd, refreshCmd);
}
