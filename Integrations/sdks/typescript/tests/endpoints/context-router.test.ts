import { describe, expect, it, vi } from "vitest";
import type { ClearContextUseCase } from "../../src/abstractions/clear-context-use-case.js";
import type { GenerateClassesUseCase } from "../../src/abstractions/generate-classes-use-case.js";
import type { GetContextGraphUseCase } from "../../src/abstractions/get-context-graph-use-case.js";
import type { GetContextUseCase } from "../../src/abstractions/get-context-use-case.js";
import type { LinkContextUseCase } from "../../src/abstractions/link-context-use-case.js";
import type { LoadContextUseCase } from "../../src/abstractions/load-context-use-case.js";
import { makeContextRouter } from "../../src/_internal/endpoints/context-router.js";

function createRouter(overrides?: {
  getContext?: GetContextUseCase;
  loadContext?: LoadContextUseCase;
  linkContext?: LinkContextUseCase;
  clearContext?: ClearContextUseCase;
  getContextGraph?: GetContextGraphUseCase;
  generateClasses?: GenerateClassesUseCase;
}) {
  const getContext: GetContextUseCase = overrides?.getContext ?? {
    get: vi.fn().mockReturnValue([{ model: "Project", pluginIds: ["p1"] }]),
  };
  const loadContext: LoadContextUseCase = overrides?.loadContext ?? {
    load: vi.fn().mockResolvedValue(undefined),
  };
  const linkContext: LinkContextUseCase = overrides?.linkContext ?? {
    link: vi.fn().mockResolvedValue(undefined),
  };
  const clearContext: ClearContextUseCase = overrides?.clearContext ?? {
    clear: vi.fn(),
  };
  const getContextGraph: GetContextGraphUseCase = overrides?.getContextGraph ?? {
    searchNodes: vi.fn().mockReturnValue({
      nodes: [{ id: "n1", label: "Node 1", type: "Class", properties: { key: "val" } }],
      edges: [{ source: "n1", target: "n2", label: "USES" }],
    }),
    getNeighbors: vi.fn().mockReturnValue({
      nodes: [{ id: "n2", label: "Node 2", type: "Class", properties: {} }],
      edges: [{ source: "n1", target: "n2", label: "USES" }],
    }),
  };
  const generateClasses: GenerateClassesUseCase = overrides?.generateClasses ?? {
    generateClasses: vi.fn().mockResolvedValue([{ name: "Model.ts", content: "export {}" }]),
  };

  const router = makeContextRouter({
    getContext: () => getContext,
    loadContext: () => loadContext,
    linkContext: () => linkContext,
    clearContext: () => clearContext,
    getContextGraph: () => getContextGraph,
    generateClasses: () => generateClasses,
  });

  return { router, getContext, loadContext, linkContext, clearContext, getContextGraph, generateClasses };
}

describe("makeContextRouter", () => {
  it("handles GET /api/context", async () => {
    const { router, getContext } = createRouter();

    const response = await router.request("/api/context");
    expect(response.status).toBe(200);

    const json = await response.json();
    expect(json).toEqual({
      data: [{ model: "Project", pluginIds: ["p1"] }],
    });
    expect(getContext.get).toHaveBeenCalled();
  });

  it("handles POST /api/context/load with valid payload", async () => {
    const { router, loadContext } = createRouter();

    const response = await router.request("/api/context/load", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        filesToLoad: {
          loaderA: ["file-1", "file-2"],
        },
      }),
    });

    expect(response.status).toBe(204);
    expect(loadContext.load).toHaveBeenCalledWith({
      loaderA: [{ value: "file-1" }, { value: "file-2" }],
    });
  });

  it("handles POST /api/context/load with invalid payload returning 400", async () => {
    const { router, loadContext } = createRouter();

    const response = await router.request("/api/context/load", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        filesToLoad: "invalid",
      }),
    });

    expect(response.status).toBe(400);
    expect(loadContext.load).not.toHaveBeenCalled();
  });

  it("handles POST /api/context/link with valid payload", async () => {
    const { router, linkContext } = createRouter();

    const response = await router.request("/api/context/link", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        linkerIds: ["linker-1", "linker-2"],
      }),
    });

    expect(response.status).toBe(204);
    expect(linkContext.link).toHaveBeenCalledWith(["linker-1", "linker-2"]);
  });

  it("handles POST /api/context/link with invalid payload returning 400", async () => {
    const { router, linkContext } = createRouter();

    const response = await router.request("/api/context/link", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({}),
    });

    expect(response.status).toBe(400);
    expect(linkContext.link).not.toHaveBeenCalled();
  });

  it("handles POST /api/context/clear", async () => {
    const { router, clearContext } = createRouter();

    const response = await router.request("/api/context/clear", {
      method: "POST",
    });

    expect(response.status).toBe(204);
    expect(clearContext.clear).toHaveBeenCalled();
  });

  it("handles GET /api/context/graph-nodes with query params", async () => {
    const { router, getContextGraph } = createRouter();

    const response = await router.request("/api/context/graph-nodes?query=Node&offset=2&limit=5");
    expect(response.status).toBe(200);

    const json = (await response.json()) as { nodes: unknown[]; edges: unknown[] };
    expect(json.nodes).toHaveLength(1);
    expect(json.edges).toHaveLength(1);
    expect(getContextGraph.searchNodes).toHaveBeenCalledWith("Node", 2, 5);
  });

  it("handles GET /api/context/graph-nodes/:nodeId/neighbors", async () => {
    const { router, getContextGraph } = createRouter();

    const response = await router.request("/api/context/graph-nodes/n1/neighbors");
    expect(response.status).toBe(200);

    const json = (await response.json()) as { nodes: unknown[]; edges: unknown[] };
    expect(json.nodes).toHaveLength(1);
    expect(getContextGraph.getNeighbors).toHaveBeenCalledWith("n1");
  });

  it("handles POST /api/context/generate-classes returning octet-stream binary bundle", async () => {
    const { router, generateClasses } = createRouter();

    const response = await router.request("/api/context/generate-classes", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        languages: ["typescript"],
      }),
    });

    expect(response.status).toBe(200);
    expect(response.headers.get("Content-Type")).toBe("application/octet-stream");
    expect(response.headers.get("Content-Disposition")).toBe('attachment; filename="classes.bin"');

    const buffer = await response.arrayBuffer();
    expect(buffer.byteLength).toBeGreaterThan(0);
    expect(generateClasses.generateClasses).toHaveBeenCalledWith(["typescript"]);
  });

  it("handles POST /api/context/generate-classes with invalid body returning 400", async () => {
    const { router, generateClasses } = createRouter();

    const response = await router.request("/api/context/generate-classes", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        languages: "not-an-array",
      }),
    });

    expect(response.status).toBe(400);
    expect(generateClasses.generateClasses).not.toHaveBeenCalled();
  });
});
