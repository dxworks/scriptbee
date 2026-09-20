import { describe, expect, it, vi } from "vitest";
import { success } from "../src/abstractions/install-plugin-use-case.js";
import {
  createAnalysisSdkRouter,
  type AnalysisSdkContainer,
} from "../src/extensions.js";

describe("createAnalysisSdkRouter", () => {
  it("assembles all routers and serves endpoints via the container", async () => {
    const container: AnalysisSdkContainer = {
      runAnalysis: {
        run: vi.fn().mockResolvedValue({
          id: { value: "analysis-1" },
          projectId: { value: "proj-1" },
          scriptId: { value: "script-1" },
          status: { value: "Finished" },
          creationDate: new Date("2026-01-01T00:00:00.000Z"),
        }),
      },
      getContext: {
        get: vi.fn().mockReturnValue([]),
      },
      loadContext: {
        load: vi.fn().mockResolvedValue(undefined),
      },
      linkContext: {
        link: vi.fn().mockResolvedValue(undefined),
      },
      clearContext: {
        clear: vi.fn(),
      },
      getContextGraph: {
        searchNodes: vi.fn().mockReturnValue({ nodes: [], edges: [] }),
        getNeighbors: vi.fn().mockReturnValue({ nodes: [], edges: [] }),
      },
      generateClasses: {
        generateClasses: vi.fn().mockResolvedValue([]),
      },
      getInstalledPlugins: {
        get: vi.fn().mockReturnValue([]),
      },
      installPlugin: {
        installPlugin: vi.fn().mockReturnValue(success()),
      },
      uninstallPlugin: {
        uninstallPlugin: vi.fn(),
      },
    };

    const app = createAnalysisSdkRouter(container);

    const postAnalyses = await app.request("/api/analyses", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ projectId: "proj-1", scriptId: "script-1" }),
    });
    expect(postAnalyses.status).toBe(202);

    const getContext = await app.request("/api/context");
    expect(getContext.status).toBe(200);

    const getPlugins = await app.request("/api/plugins");
    expect(getPlugins.status).toBe(200);

    const postClear = await app.request("/api/context/clear", {
      method: "POST",
    });
    expect(postClear.status).toBe(204);

    const deletePlugin = await app.request("/api/plugins/p1?version=1.0.0", {
      method: "DELETE",
    });
    expect(deletePlugin.status).toBe(204);
  });
});
