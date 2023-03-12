import { MarketplaceProject, MarketplaceProjectType, PluginVersion } from '../src/app/plugins/services/marketplace-project';

export function createMarketplacePlugin(
  id: string,
  name: string,
  type: MarketplaceProjectType = 'Plugin',
  versions: PluginVersion[] = [],
  authors: string[] = [],
  description = ''
): MarketplaceProject {
  return {
    id,
    type,
    name,
    versions,
    authors,
    description,
  };
}

export function createPluginVersion(installed: boolean, version = '1.0.0'): PluginVersion {
  return {
    version,
    installed,
    extensionPointVersions: [
      {
        version: '1.0.0',
        kind: 'Loader',
      },
    ],
  };
}
