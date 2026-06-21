export type PluginKind = 'ScriptGenerator' | 'ScriptRunner' | 'HelperFunctions' | 'Loader' | 'Linker' | 'UI';

export interface Plugin {
  apiVersion: string;
  name: string;
  description: string;
  author: string;
  extensionPoints: ExtensionPoint[];
}

type ExtensionPoint =
  | HelperFunctionsExtensionPoint
  | ScriptGeneratorExtensionPoint
  | ScriptRunnerExtensionPoint
  | LoaderExtensionPoint
  | LinkerExtensionPoint
  | UIExtensionPoint;

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
  remoteName: string;
  remoteEntry: string;
  outlets: UIExtensionPointOutlet[];
}

export type UIExtensionPointOutlet = TopNavigationBarExtensionPointOutlet | SidePanelExtensionPointOutlet | FilePreviewExtensionPointOutlet;

interface BaseUIExtensionPointOutlet {
  type: string;
}

export interface RoutingExtensionPointOutlet extends BaseUIExtensionPointOutlet {
  exposedModule: string;
  label: string;
  path: string;
  nested: boolean | undefined;
  componentName: string | undefined;
}

export interface TopNavigationBarExtensionPointOutlet extends RoutingExtensionPointOutlet {
  type: 'top-navigation-bar';
}

export interface SidePanelExtensionPointOutlet extends RoutingExtensionPointOutlet {
  type: 'side-panel';
  icon: string;
}

export interface FilePreviewExtensionPointOutlet extends BaseUIExtensionPointOutlet {
  type: 'file-previewer';
  exposedModule: string;
  label: string;
  componentName: string | undefined;
  icon: string;
  supportedFileExtensions: string[] | undefined;
}
