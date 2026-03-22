export type MarketplacePluginType = 'Plugin' | 'Bundle';

export interface PluginVersion {
  version: string;
  installed: boolean;
  publishDate: string;
}

export interface ExtensionPoint {
  kind: string;
  language?: string;
  extension?: string;
}

export interface BundleItem {
  id: string;
  name: string;
}

export interface MarketplacePlugin {
  id: string;
  name: string;
  type: MarketplacePluginType;
  description: string;
  authors: string[];
  latestVersion?: string;
  installedVersion?: string;
}

export interface MarketplacePluginWithDetails extends MarketplacePlugin {
  versions: PluginVersion[];
  bundleItems?: BundleItem[];
  sourceCode?: string;
  manifest?: string;
  site?: string;
  license?: string;
  tags?: string[];
  technologies?: string[];
  extensionPoints?: ExtensionPoint[];
}
