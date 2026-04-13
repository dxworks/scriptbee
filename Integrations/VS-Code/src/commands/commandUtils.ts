export type CommandConnectionArg = string | { connection?: { id: string } } | undefined;

export function getConnectionId(arg: CommandConnectionArg): string | undefined {
  if (typeof arg === 'string') {
    return arg;
  }

  if (arg && typeof arg === 'object' && 'connection' in arg && arg.connection) {
    return arg.connection.id;
  }

  return undefined;
}
