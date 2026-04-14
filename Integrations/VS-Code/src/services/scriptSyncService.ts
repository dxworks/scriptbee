import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs/promises';
import { createScript, deleteProjectFile, getProjectFiles, getScriptContent, updateScriptContent, WebProjectFileNode } from '../api/projectFiles';
import { connectionService } from './connectionService';
import { getProjectSrcPath } from '../utils/workspaceUtils';
import { Dirent } from 'fs';
import { storage } from '../utils/storage';
import { showErrorWithCopy } from '../utils/errorUtils';
import { logger } from '../utils/logger';
import { liveUpdatesService } from './liveUpdatesService';

export class ScriptSyncService {
  public async sync(connectionId: string): Promise<void> {
    await this.pull(connectionId);
    await this.push(connectionId);
  }

  public async pull(connectionId: string): Promise<void> {
    const { url, projectId } = await this.getConnectionOrThrow(connectionId);
    const srcPath = getProjectSrcPath(projectId!);
    await fs.mkdir(srcPath, { recursive: true });

    try {
      await this.pullRecursive(url, projectId!, undefined, srcPath);
    } catch (error) {
      await showErrorWithCopy('Failed to pull scripts', error);
      throw error;
    }
  }

  private async pullRecursive(baseUrl: string, projectId: string, parentId: string | undefined, currentLocalPath: string): Promise<void> {
    let offset = 0;
    const limit = 50;
    let hasMore = true;
    const remoteNodes: WebProjectFileNode[] = [];

    while (hasMore) {
      const response = await getProjectFiles(baseUrl, projectId, parentId, offset, limit);
      remoteNodes.push(...response.data);
      offset += response.data.length;
      hasMore = offset < response.totalCount && response.data.length > 0;
    }

    for (const node of remoteNodes) {
      const localPath = path.join(currentLocalPath, node.name);
      const fileUri = vscode.Uri.file(localPath);
      await storage.saveScriptMeta(fileUri, { id: node.id, type: node.type });
      liveUpdatesService.updateCacheEntry(node.id, fileUri);

      if (node.type === 'folder') {
        await fs.mkdir(localPath, { recursive: true });
        await this.pullRecursive(baseUrl, projectId, node.id, localPath);
      } else {
        await this.pullFileContent(baseUrl, projectId, node.id, localPath);
      }
    }

    await this.performOrphanCleanup(currentLocalPath, remoteNodes);
  }

  private async performOrphanCleanup(currentLocalPath: string, remoteNodes: WebProjectFileNode[]): Promise<void> {
    if (!(await this.exists(currentLocalPath))) {
      return;
    }

    const localEntries = await fs.readdir(currentLocalPath, { withFileTypes: true });
    const remoteIds = new Set(remoteNodes.map((n) => n.id));

    for (const entry of localEntries) {
      if (entry.name.endsWith('.sb.meta')) {
        continue;
      }

      const localPath = path.join(currentLocalPath, entry.name);
      const fileUri = vscode.Uri.file(localPath);
      const meta = await storage.getScriptMeta(fileUri);

      if (meta && !remoteIds.has(meta.id)) {
        try {
          await storage.deleteScriptMeta(fileUri);
          if (entry.isDirectory()) {
            await fs.rm(localPath, { recursive: true, force: true });
          } else {
            await fs.unlink(localPath);
          }
          logger.log(`Cleaned up orphaned local file/folder: ${localPath}`);
        } catch (error) {
          logger.error(`Failed to cleanup orphaned local file ${localPath}`, error);
        }
      }
    }
  }

  public async pullFileByUri(uri: vscode.Uri): Promise<void> {
    const { url, projectId, srcPath, localPath } = await this.getConnectionAndPaths(uri);

    const meta = await storage.getScriptMeta(uri);
    if (!meta) {
      throw new Error('This file is not tracked by ScriptBee. Push it first to server to create a remote counterpart.');
    }

    logger.log(`Pulling single file: ${path.relative(srcPath, localPath)} (ID: ${meta.id}) for project ${projectId}`);

    await this.pullFileContent(url, projectId!, meta.id, localPath);
  }

  public async pushFileByUri(uri: vscode.Uri): Promise<void> {
    const { url, projectId, srcPath, localPath } = await this.getConnectionAndPaths(uri);

    const relativePath = path.relative(srcPath, localPath).replace(/\\/g, '/');
    const fileName = path.basename(localPath);

    logger.log(`Pushing single file: ${relativePath} for project ${projectId}`);

    const meta = await storage.getScriptMeta(uri);
    let remoteNode: WebProjectFileNode | undefined;

    if (meta) {
      remoteNode = {
        id: meta.id,
        name: fileName,
        type: meta.type,
        path: relativePath,
        absolutePath: localPath,
        hasChildren: meta.type === 'folder',
      };
    }

    await this.pushFile(url, projectId!, srcPath, relativePath, fileName, remoteNode);
  }

  public async deleteFileByUri(fileUri: vscode.Uri) {
    await storage.deleteScriptMeta(fileUri);
    await fs.rm(fileUri.fsPath, { recursive: true, force: true });
  }

  public async push(connectionId: string): Promise<void> {
    const { url, projectId } = await this.getConnectionOrThrow(connectionId);
    const srcPath = getProjectSrcPath(projectId!);
    if (!(await this.exists(srcPath))) {
      throw new Error(`Project source folder does not exist: ${srcPath}`);
    }

    try {
      await this.pushRecursive(url, projectId!, srcPath, '', undefined);
    } catch (error) {
      void showErrorWithCopy('Failed to push scripts', error);
      throw error;
    }
  }

  private async pullFileContent(baseUrl: string, projectId: string, fileId: string, localPath: string): Promise<void> {
    try {
      const content = await getScriptContent(baseUrl, projectId, fileId);
      await fs.writeFile(localPath, content, 'utf8');
    } catch (error) {
      logger.error(`Failed to pull script content from ${fileId}`, error);
      throw error;
    }
  }

  private async getConnectionAndPaths(uri: vscode.Uri) {
    const connectionId = connectionService.getActiveConnectionId();
    if (!connectionId) {
      throw new Error('No active connection found');
    }

    const { url, projectId } = await this.getConnectionOrThrow(connectionId);
    const srcPath = path.normalize(getProjectSrcPath(projectId!));
    const localPath = path.normalize(uri.fsPath);

    if (!localPath.toLowerCase().startsWith(srcPath.toLowerCase())) {
      throw new Error('File is not within the current project source folder');
    }
    return { url, projectId: projectId!, srcPath, localPath };
  }

  private async pushRecursive(baseUrl: string, projectId: string, srcRoot: string, relativeDirPath: string, folderId: string | undefined): Promise<void> {
    const currentLocalPath = path.join(srcRoot, relativeDirPath);
    const localEntries = await fs.readdir(currentLocalPath, { withFileTypes: true });
    const parentId = folderId !== undefined ? folderId : await this.getRemoteParentId(srcRoot, relativeDirPath);

    let remoteNodes: WebProjectFileNode[] = [];
    try {
      remoteNodes = (await getProjectFiles(baseUrl, projectId, parentId, 0, 1000)).data;
    } catch (error: any) {
      if (error?.response?.status === 404) {
        remoteNodes = [];
      } else {
        throw error;
      }
    }

    if (this.isPushSafetyGuardTriggered(relativeDirPath, parentId)) {
      logger.log(`Skipping remote deletion check for untracked directory: ${relativeDirPath}`);
    } else {
      await this.handleDeletions(baseUrl, projectId, currentLocalPath, localEntries, remoteNodes);
    }

    for (const entry of localEntries) {
      if (!entry.name.endsWith('.sb.meta')) {
        await this.handlePushEntry(baseUrl, projectId, srcRoot, relativeDirPath, entry, remoteNodes);
      }
    }
  }

  private isPushSafetyGuardTriggered(relativeDirPath: string, parentId: string | undefined): boolean {
    return relativeDirPath !== '' && parentId === undefined;
  }

  private async handleDeletions(
    baseUrl: string,
    projectId: string,
    localDir: string,
    localEntries: Dirent[],
    remoteNodes: WebProjectFileNode[]
  ): Promise<void> {
    const localNames = new Set(localEntries.map((f) => f.name));
    for (const remoteNode of remoteNodes) {
      if (!localNames.has(remoteNode.name)) {
        try {
          await deleteProjectFile(baseUrl, projectId, remoteNode.id);
          const fileUri = vscode.Uri.file(path.join(localDir, remoteNode.name));
          await this.deleteFileByUri(fileUri);
        } catch (error) {
          logger.error(`Failed to delete remote node ${remoteNode.name}`, error);
          void showErrorWithCopy(`Failed to delete remote node ${remoteNode.name}`, error);
        }
      }
    }
  }

  private async handlePushEntry(
    baseUrl: string,
    projectId: string,
    srcRoot: string,
    relativeDir: string,
    entry: Dirent,
    remoteNodes: WebProjectFileNode[]
  ): Promise<void> {
    const relativePath = path.join(relativeDir, entry.name).replace(/\\/g, '/');
    const localPath = path.join(srcRoot, relativePath);
    const fileUri = vscode.Uri.file(localPath);
    const meta = await storage.getScriptMeta(fileUri);
    const remoteNode = meta ? remoteNodes.find((n) => n.id === meta.id) : remoteNodes.find((n) => n.name === entry.name);

    if (entry.isDirectory()) {
      await this.pushRecursive(baseUrl, projectId, srcRoot, relativePath, remoteNode?.id);
    } else {
      await this.pushFile(baseUrl, projectId, srcRoot, relativePath, entry.name, remoteNode);
    }
  }

  private async pushFile(
    baseUrl: string,
    projectId: string,
    srcRoot: string,
    relativePath: string,
    fileName: string,
    remoteNode?: WebProjectFileNode
  ): Promise<void> {
    const localPath = path.join(srcRoot, relativePath);
    const fileUri = vscode.Uri.file(localPath);
    try {
      const content = await fs.readFile(localPath, 'utf8');
      if (remoteNode) {
        await updateScriptContent(baseUrl, projectId, remoteNode.id, content);
        await this.ensureMeta(fileUri, remoteNode.id, 'file');
      } else {
        const lang = this.getLanguage(fileName);
        const script = await createScript(baseUrl, projectId, relativePath, lang);
        await storage.saveScriptMeta(fileUri, { id: script.id, type: 'file' });
        liveUpdatesService.updateCacheEntry(script.id, fileUri);
        await updateScriptContent(baseUrl, projectId, script.id, content);
      }
    } catch (error) {
      logger.error(`Failed to push script ${fileName}`, error);
      throw error;
    }
  }

  private async getRemoteParentId(srcRoot: string, relativeDirPath: string): Promise<string | undefined> {
    if (relativeDirPath === '') {
      return undefined;
    }
    const fileUri = vscode.Uri.file(path.join(srcRoot, relativeDirPath));
    const meta = await storage.getScriptMeta(fileUri);
    return meta?.id;
  }

  private async ensureMeta(fileUri: vscode.Uri, id: string, type: 'file' | 'folder'): Promise<void> {
    if (!(await storage.getScriptMeta(fileUri))) {
      await storage.saveScriptMeta(fileUri, { id, type });
    }
  }

  private async getConnectionOrThrow(connectionId: string) {
    const connections = await connectionService.getConnections();
    const connection = connections.find((c) => c.id === connectionId);
    if (!connection || !connection.projectId) {
      throw new Error('No project selected');
    }
    return connection;
  }

  private getLanguage(filename: string): string {
    const ext = path.extname(filename).toLowerCase();
    const map: Record<string, string> = { '.cs': 'csharp', '.py': 'python', '.js': 'javascript' };
    return map[ext] || 'javascript';
  }

  private async exists(p: string): Promise<boolean> {
    try {
      await fs.access(p);
      return true;
    } catch {
      return false;
    }
  }
}

export const scriptSyncService = new ScriptSyncService();
