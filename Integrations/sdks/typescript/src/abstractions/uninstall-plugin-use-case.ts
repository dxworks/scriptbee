import type { PluginId } from "../domain/plugins.js";

export interface UninstallPluginUseCase {
  uninstallPlugin(pluginId: PluginId): void;
}
