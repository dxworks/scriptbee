export const SDK_VERSION = "1.0.0";

export type {
  RunAnalysisCommand,
  RunAnalysisUseCase,
} from "./abstractions/run-analysis-use-case.js";
export type { GetContextUseCase } from "./abstractions/get-context-use-case.js";
export type { LoadContextUseCase } from "./abstractions/load-context-use-case.js";
export type { LinkContextUseCase } from "./abstractions/link-context-use-case.js";
export type { ClearContextUseCase } from "./abstractions/clear-context-use-case.js";
export type { GetContextGraphUseCase } from "./abstractions/get-context-graph-use-case.js";
export type { GenerateClassesUseCase } from "./abstractions/generate-classes-use-case.js";
export type { GetInstalledPluginsUseCase } from "./abstractions/get-installed-plugins-use-case.js";
export type {
  InstallPluginResult,
  InstallPluginUseCase,
  Success,
} from "./abstractions/install-plugin-use-case.js";
export { success } from "./abstractions/install-plugin-use-case.js";
export type { UninstallPluginUseCase } from "./abstractions/uninstall-plugin-use-case.js";
export type {
  InvalidPluginError,
  PluginInstallationError,
} from "./abstractions/errors.js";
export {
  invalidPluginError,
  pluginInstallationError,
} from "./abstractions/errors.js";

export type {
  AnalysisId,
  AnalysisInfo,
  AnalysisStatus,
  ResultId,
  ResultSummary,
} from "./domain/analysis.js";
export { ResultType, isRunning } from "./domain/analysis.js";
export type { SampleCodeFile } from "./domain/code-generation.js";
export type {
  ContextGraphEdge,
  ContextGraphNode,
  ContextGraphResult,
  ContextSlice,
} from "./domain/context.js";
export type {
  FileId,
  InstanceId,
  ProjectId,
  ScriptId,
} from "./domain/project.js";
export type {
  Plugin,
  PluginExtensionPoint,
  PluginId,
  PluginManifest,
} from "./domain/plugins.js";
export { PluginKind } from "./domain/plugins.js";

export type { AnalysisResultService } from "./results/analysis-result-service.js";
export { DefaultAnalysisResultService } from "./results/analysis-result-service.js";
export type { ScriptResultsStore } from "./results/script-results-store.js";

export { FileBundler } from "./file-bundler.js";

export type { AnalysisSdkContainer } from "./extensions.js";
export { createAnalysisSdkRouter } from "./extensions.js";
