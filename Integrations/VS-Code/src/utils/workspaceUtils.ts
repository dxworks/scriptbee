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

export function getProjectSrcPath(projectId: string): string {
  return path.join(getWorkspaceRoot(), 'projects', projectId, 'src');
}

export function getProjectGeneratedPath(projectId: string): string {
  return path.join(getWorkspaceRoot(), 'projects', projectId, '.generated');
}
