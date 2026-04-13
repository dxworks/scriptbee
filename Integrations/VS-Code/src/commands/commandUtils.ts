import { ConnectionItem } from '../views/ScriptBeeTreeView';

export type CommandConnectionArg = string | ConnectionItem | undefined;

export function getConnectionId(arg: CommandConnectionArg): string | undefined {
  if (typeof arg === 'string') {
    return arg;
  }

  if (arg instanceof ConnectionItem) {
    return arg.connection.id;
  }

  return undefined;
}
