import { PluginKind } from "./plugin";

export interface MarketplacePlugin {
  id: string;
  name: string;
  authors: string[];
  description: string;
  versions: PluginVersion[];
}

export interface PluginVersion {
  version: string;
  extensionPointVersions: ExtensionPointVersion[];
  installed: boolean;
}

export interface ExtensionPointVersion {
  kind: PluginKind;
  version: string;
}
