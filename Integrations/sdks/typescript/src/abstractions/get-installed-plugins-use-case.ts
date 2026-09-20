import type { Plugin } from "../domain/plugins.js";

export interface GetInstalledPluginsUseCase {
  get(): Plugin[];
}
