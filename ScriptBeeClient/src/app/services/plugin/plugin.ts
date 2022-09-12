export type Plugin = BasicPlugin | UIPlugin;

type PluginKind = 'UI' | 'Loader' | 'Linker';

export interface BasicPlugin {
  apiVersion: string;
  kind: PluginKind;
  metadata: PluginMetadata;
}

export interface PluginMetadata {
  name: string;
  entryPoint: string;
  version: string;
  description: string;
  author: string;
}

type UIPluginType = 'Result';

export interface UIPlugin extends BasicPlugin {
  kind: 'UI';
  spec: UIPluginSpec;
}


export interface UIPluginSpec {
  remoteEntry: string;
  exposedModule: string;
  componentName: string;
  uiPluginType: UIPluginType;
}
