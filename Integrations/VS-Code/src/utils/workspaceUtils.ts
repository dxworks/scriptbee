import * as vscode from 'vscode';
import * as path from 'path';
import * as os from 'os';

export function getWorkspaceRoot(): string {
  const config = vscode.workspace.getConfiguration('scriptbee');
  let root = config.get<string>('workspaceRoot') || '~/.scriptbee';

  if (root.startsWith('~/')) {
    root = path.join(os.homedir(), root.slice(2));
  } else if (root === '~') {
    root = os.homedir();
  }

  return path.resolve(root);
}

export function getProjectRootPath(projectId: string): string {
  return path.join(getWorkspaceRoot(), 'projects', projectId);
}

export function getProjectSrcPath(projectId: string): string {
  return path.join(getWorkspaceRoot(), 'projects', projectId, 'src');
}

export function getProjectGeneratedPath(projectId: string): string {
  return path.join(getWorkspaceRoot(), 'projects', projectId, '.generated');
}

export async function hideMetaFiles() {
  const config = vscode.workspace.getConfiguration('files');
  const exclude = config.get<Record<string, boolean>>('exclude') || {};

  if (exclude['**/*.sb.meta'] === true) {
    return;
  }

  const newExclude = { ...exclude, '**/*.sb.meta': true };

  try {
    if (vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length > 0) {
      await config.update('exclude', newExclude, vscode.ConfigurationTarget.Workspace);
    } else {
      await config.update('exclude', newExclude, vscode.ConfigurationTarget.Global);
    }
  } catch (error) {
    console.warn('Failed to update workspace exclude bindings:', error);
  }
}
