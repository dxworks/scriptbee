import { z } from "zod";

export const PluginKind = {
  PLUGIN: "Plugin",
  LOADER: "Loader",
  LINKER: "Linker",
  SCRIPT_GENERATOR: "ScriptGenerator",
  SCRIPT_RUNNER: "ScriptRunner",
  HELPER_FUNCTIONS: "HelperFunctions",
  UI: "Ui",
} as const;
export type PluginKind = (typeof PluginKind)[keyof typeof PluginKind];

export const OutletType = {
  TOP_NAVIGATION_BAR: "TopNavigationBar",
  SIDE_PANEL: "SidePanel",
  FILE_PREVIEWER: "FilePreviewer",
} as const;
export type OutletType = (typeof OutletType)[keyof typeof OutletType];

export const PluginIdSchema = z.object({
  name: z.string(),
  version: z.string(),
});
export type PluginId = z.infer<typeof PluginIdSchema>;

export const TopNavigationBarOutletSchema = z.object({
  type: z.literal(OutletType.TOP_NAVIGATION_BAR),
  exposedModule: z.string(),
  label: z.string(),
  path: z.string(),
  nested: z.boolean().nullable().optional(),
  componentName: z.string().nullable().optional(),
});
export type TopNavigationBarOutlet = z.infer<
  typeof TopNavigationBarOutletSchema
>;

export const SidePanelOutletSchema = z.object({
  type: z.literal(OutletType.SIDE_PANEL),
  exposedModule: z.string(),
  label: z.string(),
  path: z.string(),
  nested: z.boolean().nullable().optional(),
  componentName: z.string().nullable().optional(),
  icon: z.string().default(""),
});
export type SidePanelOutlet = z.infer<typeof SidePanelOutletSchema>;

export const FilePreviewerOutletSchema = z.object({
  type: z.literal(OutletType.FILE_PREVIEWER),
  exposedModule: z.string(),
  label: z.string(),
  componentName: z.string().nullable().optional(),
  icon: z.string().nullable().optional(),
  supportedFileExtensions: z.array(z.string()).nullable().optional(),
});
export type FilePreviewerOutlet = z.infer<typeof FilePreviewerOutletSchema>;

export const UiPluginExtensionPointOutletSchema = z.discriminatedUnion("type", [
  TopNavigationBarOutletSchema,
  SidePanelOutletSchema,
  FilePreviewerOutletSchema,
]);
export type UiPluginExtensionPointOutlet = z.infer<
  typeof UiPluginExtensionPointOutletSchema
>;

export const NestedPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.PLUGIN),
  entryPoint: z.string(),
  version: z.string(),
});

export const LoaderPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.LOADER),
  entryPoint: z.string(),
  version: z.string(),
});

export const LinkerPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.LINKER),
  entryPoint: z.string(),
  version: z.string(),
});

export const ScriptGeneratorPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.SCRIPT_GENERATOR),
  entryPoint: z.string(),
  version: z.string(),
  language: z.string(),
  extension: z.string(),
});

export const ScriptRunnerPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.SCRIPT_RUNNER),
  entryPoint: z.string(),
  version: z.string(),
  language: z.string(),
  extension: z.string(),
});

export const HelperFunctionsPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.HELPER_FUNCTIONS),
  entryPoint: z.string(),
  version: z.string(),
});

export const UiPluginExtensionPointSchema = z.object({
  kind: z.literal(PluginKind.UI),
  entryPoint: z.string(),
  version: z.string(),
  remoteName: z.string(),
  remoteEntry: z.string(),
  outlets: z.array(UiPluginExtensionPointOutletSchema).default([]),
});

export const PluginExtensionPointSchema = z.discriminatedUnion("kind", [
  NestedPluginExtensionPointSchema,
  LoaderPluginExtensionPointSchema,
  LinkerPluginExtensionPointSchema,
  ScriptGeneratorPluginExtensionPointSchema,
  ScriptRunnerPluginExtensionPointSchema,
  HelperFunctionsPluginExtensionPointSchema,
  UiPluginExtensionPointSchema,
]);
export type PluginExtensionPoint = z.infer<typeof PluginExtensionPointSchema>;

export const PluginManifestSchema = z.object({
  apiVersion: z.string(),
  name: z.string(),
  description: z.string().nullable().optional(),
  author: z.string().nullable().optional(),
  extensionPoints: z.array(PluginExtensionPointSchema).default([]),
});
export type PluginManifest = z.infer<typeof PluginManifestSchema>;

export const PluginSchema = z.object({
  folderPath: z.string(),
  id: PluginIdSchema,
  manifest: PluginManifestSchema,
});
export type Plugin = z.infer<typeof PluginSchema>;
