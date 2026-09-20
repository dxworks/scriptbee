import { Hono } from "hono";
import { makeAnalysesRouter } from "./_internal/endpoints/analyses-router.js";
import { makeContextRouter } from "./_internal/endpoints/context-router.js";
import { makePluginsRouter } from "./_internal/endpoints/plugins-router.js";
import type { ClearContextUseCase } from "./abstractions/clear-context-use-case.js";
import type { GenerateClassesUseCase } from "./abstractions/generate-classes-use-case.js";
import type { GetContextGraphUseCase } from "./abstractions/get-context-graph-use-case.js";
import type { GetContextUseCase } from "./abstractions/get-context-use-case.js";
import type { GetInstalledPluginsUseCase } from "./abstractions/get-installed-plugins-use-case.js";
import type { InstallPluginUseCase } from "./abstractions/install-plugin-use-case.js";
import type { LinkContextUseCase } from "./abstractions/link-context-use-case.js";
import type { LoadContextUseCase } from "./abstractions/load-context-use-case.js";
import type { RunAnalysisUseCase } from "./abstractions/run-analysis-use-case.js";
import type { UninstallPluginUseCase } from "./abstractions/uninstall-plugin-use-case.js";

export interface AnalysisSdkContainer {
  readonly runAnalysis: RunAnalysisUseCase;
  readonly getContext: GetContextUseCase;
  readonly loadContext: LoadContextUseCase;
  readonly linkContext: LinkContextUseCase;
  readonly clearContext: ClearContextUseCase;
  readonly getContextGraph: GetContextGraphUseCase;
  readonly generateClasses: GenerateClassesUseCase;
  readonly getInstalledPlugins: GetInstalledPluginsUseCase;
  readonly installPlugin: InstallPluginUseCase;
  readonly uninstallPlugin: UninstallPluginUseCase;
}

export function createAnalysisSdkRouter(container: AnalysisSdkContainer): Hono {
  const app = new Hono();

  app.route(
    "/",
    makeAnalysesRouter(() => container.runAnalysis),
  );
  app.route(
    "/",
    makeContextRouter({
      getContext: () => container.getContext,
      loadContext: () => container.loadContext,
      linkContext: () => container.linkContext,
      clearContext: () => container.clearContext,
      getContextGraph: () => container.getContextGraph,
      generateClasses: () => container.generateClasses,
    }),
  );
  app.route(
    "/",
    makePluginsRouter({
      getInstalledPlugins: () => container.getInstalledPlugins,
      installPlugin: () => container.installPlugin,
      uninstallPlugin: () => container.uninstallPlugin,
    }),
  );

  return app;
}
