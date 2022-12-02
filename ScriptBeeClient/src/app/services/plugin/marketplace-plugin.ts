import { PluginKind } from "./plugin";

export interface MarketplacePlugin {
  id: string;
  name: string;
  author: string;
  description: string;
  versions: Versions;
}

export interface PluginVersion {
  extensionPointVersions: ExtensionPointVersion[];
  installed: boolean;
}

export interface ExtensionPointVersion {
  kind: PluginKind;
  version: string;
}

type Versions = { [version: string]: PluginVersion };

