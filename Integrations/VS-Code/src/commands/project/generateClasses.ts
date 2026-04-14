import * as vscode from 'vscode';
import { connectionService } from '../../services/connectionService';
import { generateClassesService } from '../../services/generateClassesService';
import { Connection } from '../../utils/storage';
import { CommandConnectionArg, getConnectionId } from '../commandUtils';
import { logger } from '../../utils/logger';
import { showErrorWithCopy } from '../../utils/errorUtils';

export async function generateClasses(connectionArg?: CommandConnectionArg) {
  let connection: Connection | undefined;
  const connectionId = getConnectionId(connectionArg);

  if (connectionId) {
    const connections = await connectionService.getConnections();
    connection = connections.find((c) => c.id === connectionId);
  } else {
    connection = await connectionService.getActiveConnection();
  }

  if (!connection || !connection.projectId || !connection.instanceId) {
    vscode.window.showErrorMessage('Please select a project and an instance first.');
    return;
  }

  try {
    await vscode.window.withProgress(
      {
        location: vscode.ProgressLocation.Notification,
        title: 'ScriptBee',
        cancellable: false,
      },
      async (progress) => {
        progress.report({ message: `Generating classes for ${connection?.projectId}...` });
        await generateClassesService.generate(connection!.url, connection!.projectId!, connection!.instanceId!, []);
      }
    );

    vscode.window.showInformationMessage(`Classes successfully generated in .generated folder.`);
  } catch (error) {
    logger.error(`Failed to generate classes for ${connection.projectId}`, error);
    await showErrorWithCopy(`Failed to generate classes for ${connection.projectId}`, error);
  }
}
