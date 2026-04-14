import * as vscode from 'vscode';
import { getScriptContent } from '../api/projectFiles';
import { logger } from '../utils/logger';

export class RemoteScriptProvider implements vscode.TextDocumentContentProvider {
  public static scheme = 'scriptbee-remote';

  public async provideTextDocumentContent(uri: vscode.Uri): Promise<string> {
    try {
      const pathSegments = uri.path.split('/').filter((s) => s.length > 0);
      if (pathSegments.length < 2) {
        throw new Error('Invalid remote script URI');
      }

      const projectId = pathSegments[0];
      const fileId = pathSegments[1];

      const params = new URLSearchParams(uri.query);
      const baseUrl = params.get('baseUrl');

      if (!baseUrl) {
        throw new Error('Base URL is missing in the remote script URI query parameters');
      }

      return await getScriptContent(baseUrl, projectId, fileId);
    } catch (error) {
      logger.error(`Failed to load remote script content for ${uri.toString()}`, error);
      return `/* Failed to load remote script: ${(error as any).message || String(error)} */`;
    }
  }
}
