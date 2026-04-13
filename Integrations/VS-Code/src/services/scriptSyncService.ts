import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs/promises';
import { getProjectFiles, getScriptContent, updateScriptContent, createScript, WebProjectFileNode } from '../api/projectFiles';
import { connectionService } from './connectionService';
import { getProjectSrcPath } from '../utils/workspaceUtils';
import { Dirent } from 'fs';

export class ScriptSyncService {
  public async sync(connectionId: string): Promise<void> {
    await this.pull(connectionId);
    await this.push(connectionId);
  }

  public async pull(connectionId: string): Promise<void> {
    const connection = await this.getConnectionOrThrow(connectionId);
    const srcPath = getProjectSrcPath(connection.projectId!);
    console.log('srcPath', srcPath);
    await fs.mkdir(srcPath, { recursive: true });

    await this.pullRecursive(connection.url, connection.projectId!, undefined, srcPath);
  }

  private async pullRecursive(baseUrl: string, projectId: string, parentId: string | undefined, currentLocalPath: string): Promise<void> {
    let offset = 0;
    const limit = 50;
    let hasMore = true;

    while (hasMore) {
      const response = await getProjectFiles(baseUrl, projectId, parentId, offset, limit);
      console.log('offset', response.data);
      for (const node of response.data) {
        await this.handlePullNode(baseUrl, projectId, node, currentLocalPath);
      }

      offset += response.data.length;
      hasMore = offset < response.totalCount && response.data.length > 0;
    }
  }

  private async handlePullNode(baseUrl: string, projectId: string, node: WebProjectFileNode, currentLocalPath: string): Promise<void> {
    const localPath = path.join(currentLocalPath, node.name);

    if (node.type === 'folder') {
      await fs.mkdir(localPath, { recursive: true });
      await this.pullRecursive(baseUrl, projectId, node.id, localPath);
    } else {
      try {
        console.log(baseUrl, projectId, node.id);
        const content = await getScriptContent(baseUrl, projectId, node.id);
        await fs.writeFile(localPath, content, 'utf8');
      } catch (error) {
        console.log(error);
        vscode.window.showErrorMessage(`Failed to pull script ${node.name}: ${error}`);
      }
    }
  }

  public async push(connectionId: string): Promise<void> {
    const connection = await this.getConnectionOrThrow(connectionId);
    const srcPath = getProjectSrcPath(connection.projectId!);

    if (!(await this.exists(srcPath))) {
      throw new Error(`Project source folder does not exist: ${srcPath}`);
    }

    await this.pushRecursive(connection.url, connection.projectId!, srcPath, '');
  }

  private async pushRecursive(baseUrl: string, projectId: string, srcRoot: string, relativeDirPath: string): Promise<void> {
    const currentLocalPath = path.join(srcRoot, relativeDirPath);
    const files = await fs.readdir(currentLocalPath, { withFileTypes: true });
    const parentId = await this.getRemoteParentId(baseUrl, projectId, relativeDirPath);
    const remoteNodes = (await getProjectFiles(baseUrl, projectId, parentId, 0, 1000)).data;

    for (const file of files) {
      await this.handlePushEntry(baseUrl, projectId, srcRoot, relativeDirPath, file, remoteNodes);
    }
  }

  private async getRemoteParentId(baseUrl: string, projectId: string, relativeDirPath: string): Promise<string | undefined> {
    if (relativeDirPath === '') {
      return undefined;
    }
    return await this.findRemoteFolderId(baseUrl, projectId, relativeDirPath);
  }

  private async handlePushEntry(
    baseUrl: string,
    projectId: string,
    srcRoot: string,
    relativeDirPath: string,
    file: Dirent,
    remoteNodes: WebProjectFileNode[]
  ): Promise<void> {
    const relativeFilePath = path.join(relativeDirPath, file.name).replace(/\\/g, '/');
    const remoteNode = remoteNodes.find((n) => n.name === file.name);

    if (file.isDirectory()) {
      await this.pushRecursive(baseUrl, projectId, srcRoot, relativeFilePath);
    } else {
      await this.pushFile(baseUrl, projectId, srcRoot, relativeFilePath, file.name, remoteNode);
    }
  }

  private async pushFile(
    baseUrl: string,
    projectId: string,
    srcRoot: string,
    relativeFilePath: string,
    fileName: string,
    remoteNode: WebProjectFileNode | undefined
  ): Promise<void> {
    const localPath = path.join(srcRoot, relativeFilePath);
    const content = await fs.readFile(localPath, 'utf8');

    try {
      if (remoteNode) {
        await updateScriptContent(baseUrl, projectId, remoteNode.id, content);
      } else {
        const language = this.getLanguageFromExtension(fileName);
        const newScript = await createScript(baseUrl, projectId, relativeFilePath, language);
        await updateScriptContent(baseUrl, projectId, newScript.id, content);
      }
    } catch (error) {
      vscode.window.showErrorMessage(`Failed to push script ${fileName}: ${error}`);
    }
  }

  private async getConnectionOrThrow(connectionId: string) {
    const connections = await connectionService.getConnections();
    const connection = connections.find((c) => c.id === connectionId);

    if (!connection || !connection.projectId) {
      throw new Error('No project selected for sync operation');
    }
    return connection;
  }

  private async findRemoteFolderId(baseUrl: string, projectId: string, relativeDirPath: string): Promise<string | undefined> {
    const parts = relativeDirPath.split(/[\\/]/).filter((p) => p !== '');
    let currentParentId: string | undefined;

    for (const part of parts) {
      const response = await getProjectFiles(baseUrl, projectId, currentParentId, 0, 1000);
      const folder = response.data.find((n) => n.type === 'folder' && n.name === part);
      if (!folder) {
        return undefined;
      }
      currentParentId = folder.id;
    }

    return currentParentId;
  }

  private getLanguageFromExtension(filename: string): string {
    const ext = path.extname(filename).toLowerCase();
    switch (ext) {
      case '.cs':
        return 'csharp';
      case '.py':
        return 'python';
      case '.js':
        return 'javascript';
      default:
        return 'csharp';
    }
  }

  private async exists(path: string): Promise<boolean> {
    try {
      await fs.access(path);
      return true;
    } catch {
      return false;
    }
  }
}

export const scriptSyncService = new ScriptSyncService();
