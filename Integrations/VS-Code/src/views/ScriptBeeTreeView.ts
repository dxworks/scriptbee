import * as vscode from 'vscode';
import { connectionService } from '../services/connectionService';
import { Connection } from '../utils/storage';
import * as CommandIds from '../commands/commandIds';

export class ScriptBeeTreeDataProvider implements vscode.TreeDataProvider<ScriptBeeTreeItem> {
  private _onDidChangeTreeData: vscode.EventEmitter<ScriptBeeTreeItem | undefined | void> = new vscode.EventEmitter<ScriptBeeTreeItem | undefined | void>();
  readonly onDidChangeTreeData: vscode.Event<ScriptBeeTreeItem | undefined | void> = this._onDidChangeTreeData.event;

  refresh(): void {
    this._onDidChangeTreeData.fire();
  }

  getTreeItem(element: ScriptBeeTreeItem): vscode.TreeItem {
    return element;
  }

  async getChildren(element?: ScriptBeeTreeItem): Promise<ScriptBeeTreeItem[]> {
    if (!element) {
      const connections = await connectionService.getConnections();
      const activeId = connectionService.getActiveConnectionId();

      return connections.map((c) => new ConnectionItem(c, c.id === activeId));
    }

    if (element instanceof ConnectionItem) {
      const connection = element.connection;
      const items: ScriptBeeTreeItem[] = [];

      if (connection.projectId) {
        items.push(new ProjectItem(connection.projectId));
      } else {
        items.push(new ActionItem('Select Project', CommandIds.COMMAND_SELECT_PROJECT, 'project'));
      }

      return items;
    }

    return [];
  }
}

export abstract class ScriptBeeTreeItem extends vscode.TreeItem {}

export class ConnectionItem extends ScriptBeeTreeItem {
  constructor(
    public readonly connection: Connection,
    public readonly isActive: boolean
  ) {
    super(connection.name, vscode.TreeItemCollapsibleState.Collapsed);
    this.description = connection.url;
    this.contextValue = 'connection';
    this.iconPath = new vscode.ThemeIcon(isActive ? 'check' : 'link');
    if (isActive) {
      this.tooltip = `${connection.name} (Active)`;
    }
  }
}

class ProjectItem extends ScriptBeeTreeItem {
  constructor(projectId: string) {
    super(`Project: ${projectId}`, vscode.TreeItemCollapsibleState.None);
    this.iconPath = new vscode.ThemeIcon('project');
    this.contextValue = 'project';
  }
}

class ActionItem extends ScriptBeeTreeItem {
  constructor(label: string, commandId: string, icon: string) {
    super(label, vscode.TreeItemCollapsibleState.None);
    this.iconPath = new vscode.ThemeIcon(icon);
    this.command = {
      title: label,
      command: commandId,
    };
  }
}

export function initTreeView(context: vscode.ExtensionContext) {
  const treeDataProvider = new ScriptBeeTreeDataProvider();
  const treeView = vscode.window.createTreeView(CommandIds.VIEW_ID_CONNECTIONS, {
    treeDataProvider,
  });

  const refreshCmd = vscode.commands.registerCommand(CommandIds.COMMAND_REFRESH_TREE_VIEW, () => {
    treeDataProvider.refresh();
  });

  context.subscriptions.push(treeView, refreshCmd);
  return treeDataProvider;
}
