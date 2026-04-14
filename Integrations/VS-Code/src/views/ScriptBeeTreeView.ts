import * as vscode from 'vscode';
import { connectionService } from '../services/connectionService';
import { Connection } from '../utils/storage';
import * as CommandIds from '../commands/commandIds';
import { getProjectInstances, InstanceResponse } from '../api/instances';

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
        items.push(new ProjectItem(connection.projectId, connection));
      } else {
        items.push(new ActionItem('Select Project', CommandIds.COMMAND_SELECT_PROJECT, 'project', connection));
      }

      return items;
    }

    if (element instanceof ProjectItem) {
      const connection = element.connection!;
      const instances = await getProjectInstances(connection.url, element.projectId);
      const items: ScriptBeeTreeItem[] = [];

      if (connection.instanceId) {
        const activeInstance = instances.find((i) => i.id === connection.instanceId);
        if (activeInstance) {
          items.push(new InstanceItem(activeInstance, connection, true));
        } else {
          items.push(new ActionItem('Select Instance', CommandIds.COMMAND_SELECT_INSTANCE, 'instance', connection));
        }
      } else {
        items.push(new ActionItem('Select Instance', CommandIds.COMMAND_SELECT_INSTANCE, 'instance', connection));
      }

      return items;
    }

    return [];
  }
}

export abstract class ScriptBeeTreeItem extends vscode.TreeItem {
  constructor(
    label: string,
    collapsibleState: vscode.TreeItemCollapsibleState,
    public readonly connection?: Connection
  ) {
    super(label, collapsibleState);
  }
}

export class ConnectionItem extends ScriptBeeTreeItem {
  constructor(
    public readonly connection: Connection,
    public readonly isActive: boolean
  ) {
    super(connection.name, vscode.TreeItemCollapsibleState.Collapsed, connection);
    this.description = connection.url;
    this.contextValue = isActive ? 'connectionActive' : 'connection';
    this.iconPath = new vscode.ThemeIcon(isActive ? 'check' : 'link');
    if (isActive) {
      this.tooltip = `${connection.name} (Active)`;
    }
  }
}

export class ProjectItem extends ScriptBeeTreeItem {
  constructor(
    public readonly projectId: string,
    connection: Connection
  ) {
    super(`Project: ${projectId}`, vscode.TreeItemCollapsibleState.Collapsed, connection);
    this.iconPath = new vscode.ThemeIcon('project');
    this.contextValue = 'project';
  }
}

export class InstanceItem extends ScriptBeeTreeItem {
  constructor(
    public readonly instance: InstanceResponse,
    connection: Connection,
    public readonly isActive: boolean
  ) {
    super(`Instance: ${instance.id}`, vscode.TreeItemCollapsibleState.None, connection);
    this.description = instance.status;
    this.contextValue = 'instance';
    this.iconPath = new vscode.ThemeIcon('database');
  }
}

class ActionItem extends ScriptBeeTreeItem {
  constructor(label: string, commandId: string, icon: string, connection: Connection) {
    super(label, vscode.TreeItemCollapsibleState.None, connection);
    this.iconPath = new vscode.ThemeIcon(icon);
    this.command = {
      title: label,
      command: commandId,
      arguments: [this],
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
