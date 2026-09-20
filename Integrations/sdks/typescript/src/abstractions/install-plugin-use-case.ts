import type { PluginId } from "../domain/plugins.js";
import type { InvalidPluginError, PluginInstallationError } from "./errors.js";

export interface Success {
  readonly _tag: "Success";
}

export function success(): Success {
  return { _tag: "Success" };
}

export type InstallPluginResult = Success | InvalidPluginError | PluginInstallationError;

export interface InstallPluginUseCase {
  installPlugin(pluginId: PluginId): InstallPluginResult;
}
