import type { PluginId } from "../domain/plugins.js";

export interface InvalidPluginError {
  readonly _tag: "InvalidPluginError";
  readonly id: PluginId;
}

export interface PluginInstallationError {
  readonly _tag: "PluginInstallationError";
  readonly id: PluginId;
}

export function invalidPluginError(id: PluginId): InvalidPluginError {
  return { _tag: "InvalidPluginError", id };
}

export function pluginInstallationError(id: PluginId): PluginInstallationError {
  return { _tag: "PluginInstallationError", id };
}
