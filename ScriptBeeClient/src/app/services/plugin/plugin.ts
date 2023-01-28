export type PluginKind = 'ScriptGenerator' | 'ScriptRunner' | 'HelperFunctions' | 'Loader' | 'Linker' | 'UI';

export interface Plugin {
  apiVersion: string;
  name: string;
  description: string;
  author: string;
  extensionPoints: ExtensionPoint[]
}

interface BasicExtensionPoint {
  kind: PluginKind;
  entryPoint: string;
  version: string;
}

interface ScriptGeneratorExtensionPoint extends BasicExtensionPoint {
  kind: 'ScriptGenerator';
  language: string;
  extension: string;
}

interface ScriptRunnerExtensionPoint extends BasicExtensionPoint {
  kind: 'ScriptRunner';
  language: string;
}

interface HelperFunctionsExtensionPoint extends BasicExtensionPoint {
  kind: 'HelperFunctions';
}

interface LoaderExtensionPoint extends BasicExtensionPoint {
  kind: 'Loader';
}

interface LinkerExtensionPoint extends BasicExtensionPoint {
  kind: 'Linker';
}

interface UIExtensionPoint extends BasicExtensionPoint {
  kind: 'UI';
  port: number;
  remoteEntry: string;
  exposedModule: string;
  componentName: string;
  uiPluginType: string;
}

type ExtensionPoint =
  HelperFunctionsExtensionPoint
  | ScriptGeneratorExtensionPoint
  | ScriptRunnerExtensionPoint
  | LoaderExtensionPoint
  | LinkerExtensionPoint
  | UIExtensionPoint;

