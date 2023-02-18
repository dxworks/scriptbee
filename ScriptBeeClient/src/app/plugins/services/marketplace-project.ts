import { PluginKind } from './plugin';

export type MarketplaceProjectType = 'Plugin' | 'Bundle';

export interface MarketplaceProject {
  id: string;
  name: string;
  type: MarketplaceProjectType;
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
