export type MarketplacePluginType = 'Plugin' | 'Bundle';

export interface PluginVersion {
  version: string;
  url: boolean;
  manifestUrl: string;
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

export interface InstalledPlugin {
  id: string;
  version: string;
}

export interface MarketplacePlugin {
  id: string;
  name: string;
  type: MarketplacePluginType;
  description: string;
  authors: string[];
  versions: PluginVersion[];
}

export interface MarketplacePluginWithDetails extends MarketplacePlugin {
  bundleItems?: BundleItem[];
  sourceCode?: string;
  manifest?: string;
  site?: string;
  license?: string;
  tags?: string[];
  technologies?: string[];
  extensionPoints?: ExtensionPoint[];
}
