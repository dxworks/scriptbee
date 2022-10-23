import { PluginKind } from "./plugin";

export type MarketplacePluginType = 'plugin' | 'bundle';

export interface BaseMarketplacePlugin {
  id: string;
  name: string;
  author: string;
  description: string;
  downloadUrl: string;
  type: MarketplacePluginType;
}

export interface MarketplaceSinglePlugin extends BaseMarketplacePlugin {
  type: 'plugin';
  versions: Versions;
}

export interface MarketplaceBundlePlugin extends BaseMarketplacePlugin {
  type: 'bundle';
  versions: BundleVersions;
}

interface PluginVersion {
  kinds: PluginKind[];
  installed: boolean;
}

interface BundlePlugin {
  name: string;
  version: string;
  kinds: PluginKind[];
}

interface BundleVersion {
  plugins: BundlePlugin[];
  installed: boolean;
}

type Versions = { [version: string]: PluginVersion };

type BundleVersions = { [version: string]: BundleVersion };
