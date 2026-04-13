import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs/promises';
import { getProjectFiles, getScriptContent, updateScriptContent, createScript, deleteProjectFile, WebProjectFileNode } from '../api/projectFiles';
import { connectionService } from './connectionService';
import { getProjectSrcPath } from '../utils/workspaceUtils';
import { Dirent } from 'fs';

interface ScriptMeta {
  id: string;
  type: 'file' | 'folder';
}

export class ScriptSyncService {
  public async sync(connectionId: string): Promise<void> {
    await this.pull(connectionId);
    await this.push(connectionId);
  }

  public async pull(connectionId: string): Promise<void> {
    const connection = await this.getConnectionOrThrow(connectionId);
    const srcPath = getProjectSrcPath(connection.projectId!);
    await fs.mkdir(srcPath, { recursive: true });

    await this.pullRecursive(connection.url, connection.projectId!, undefined, srcPath);
  }

  private async pullRecursive(
    baseUrl: string,
    projectId: string,
    parentId: string | undefined,
    currentLocalPath: string
  ): Promise<void> {
    let offset = 0;
    const limit = 50;
    let hasMore = true;

    while (hasMore) {
      const response = await getProjectFiles(baseUrl, projectId, parentId, offset, limit);

      for (const node of response.data) {
        await this.handlePullNode(baseUrl, projectId, node, currentLocalPath);
      }

      offset += response.data.length;
      hasMore = offset < response.totalCount && response.data.length > 0;
    }
  }

  private async handlePullNode(baseUrl: string, projectId: string, node: WebProjectFileNode, currentLocalPath: string): Promise<void> {
    const localPath = path.join(currentLocalPath, node.name);
    await this.writeMetaFile(localPath, { id: node.id, type: node.type });

    if (node.type === 'folder') {
      await fs.mkdir(localPath, { recursive: true });
      await this.pullRecursive(baseUrl, projectId, node.id, localPath);
    } else {
      try {
        const content = await getScriptContent(baseUrl, projectId, node.id);
        await fs.writeFile(localPath, content, 'utf8');
      } catch (error) {
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

  private async pushRecursive(
    baseUrl: string,
    projectId: string,
    srcRoot: string,
    relativeDirPath: string
  ): Promise<void> {
    const currentLocalPath = path.join(srcRoot, relativeDirPath);
    const localEntries = await fs.readdir(currentLocalPath, { withFileTypes: true });
    const parentId = await this.getRemoteParentId(baseUrl, projectId, srcRoot, relativeDirPath);
    const remoteNodes = (await getProjectFiles(baseUrl, projectId, parentId, 0, 1000)).data;

    await this.handleDeletions(baseUrl, projectId, currentLocalPath, localEntries, remoteNodes);

    for (const entry of localEntries) {
      if (entry.name.endsWith('.sb.meta')) {
        continue;
      }
      await this.handlePushEntry(baseUrl, projectId, srcRoot, relativeDirPath, entry, remoteNodes);
    }
  }

  private async handleDeletions(
    baseUrl: string,
    projectId: string,
    currentLocalPath: string,
    localEntries: Dirent[],
    remoteNodes: WebProjectFileNode[]
  ): Promise<void> {
    const localNames = new Set(localEntries.map(f => f.name));
    
    for (const remoteNode of remoteNodes) {
      if (!localNames.has(remoteNode.name)) {
        try {
          await deleteProjectFile(baseUrl, projectId, remoteNode.id);
          const metaPath = path.join(currentLocalPath, `${remoteNode.name}.sb.meta`);
          if (await this.exists(metaPath)) {
            await fs.unlink(metaPath);
          }
        } catch (error) {
          vscode.window.showErrorMessage(`Failed to delete remote node ${remoteNode.name}: ${error}`);
        }
      }
    }
  }

  private async handlePushEntry(
    baseUrl: string,
    projectId: string,
    srcRoot: string,
    relativeDirPath: string,
    entry: Dirent,
    remoteNodes: WebProjectFileNode[]
  ): Promise<void> {
    const relativeFilePath = path.join(relativeDirPath, entry.name).replace(/\\/g, '/');
    const localPath = path.join(srcRoot, relativeFilePath);
    const meta = await this.readMetaFile(localPath);
    
    const remoteNode = meta 
      ? remoteNodes.find(n => n.id === meta.id)
      : remoteNodes.find(n => n.name === entry.name);

    if (entry.isDirectory()) {
      await this.pushRecursive(baseUrl, projectId, srcRoot, relativeFilePath);
    } else {
      await this.pushFile(baseUrl, projectId, srcRoot, relativeFilePath, entry.name, remoteNode);
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
        if (!(await this.readMetaFile(localPath))) {
          await this.writeMetaFile(localPath, { id: remoteNode.id, type: 'file' });
        }
      } else {
        const language = this.getLanguageFromExtension(fileName);
        const newScript = await createScript(baseUrl, projectId, relativeFilePath, language);
        await this.writeMetaFile(localPath, { id: newScript.id, type: 'file' });
        await updateScriptContent(baseUrl, projectId, newScript.id, content);
      }
    } catch (error) {
      vscode.window.showErrorMessage(`Failed to push script ${fileName}: ${error}`);
    }
  }

  private async getRemoteParentId(baseUrl: string, projectId: string, srcRoot: string, relativeDirPath: string): Promise<string | undefined> {
    if (relativeDirPath === '') {
      return undefined;
    }
    const localPath = path.join(srcRoot, relativeDirPath);
    const meta = await this.readMetaFile(localPath);
    return meta?.id;
  }

  private async readMetaFile(targetPath: string): Promise<ScriptMeta | undefined> {
    const metaPath = `${targetPath}.sb.meta`;
    if (await this.exists(metaPath)) {
      try {
        const content = await fs.readFile(metaPath, 'utf8');
        return JSON.parse(content);
      } catch {
        return undefined;
      }
    }
    return undefined;
  }

  private async writeMetaFile(targetPath: string, meta: ScriptMeta): Promise<void> {
    const metaPath = `${targetPath}.sb.meta`;
    await fs.writeFile(metaPath, JSON.stringify(meta), 'utf8');
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
