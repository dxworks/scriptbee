import { Hono } from "hono";
import type { ClearContextUseCase } from "../../abstractions/clear-context-use-case.js";
import type { GenerateClassesUseCase } from "../../abstractions/generate-classes-use-case.js";
import type { GetContextGraphUseCase } from "../../abstractions/get-context-graph-use-case.js";
import type { GetContextUseCase } from "../../abstractions/get-context-use-case.js";
import type { LinkContextUseCase } from "../../abstractions/link-context-use-case.js";
import type { LoadContextUseCase } from "../../abstractions/load-context-use-case.js";
import type { FileId } from "../../domain/project.js";
import { FileBundler } from "../../file-bundler.js";
import {
  WebGenerateClassesRequestSchema,
  WebLinkContextCommandSchema,
  WebLoadContextCommandSchema,
  webContextGraphEdgeFromDomain,
  webContextGraphNodeFromDomain,
  webContextSliceFromDomain,
} from "./contracts/web-models.js";

export function makeContextRouter(options: {
  getContext: () => GetContextUseCase;
  loadContext: () => LoadContextUseCase;
  linkContext: () => LinkContextUseCase;
  clearContext: () => ClearContextUseCase;
  getContextGraph: () => GetContextGraphUseCase;
  generateClasses: () => GenerateClassesUseCase;
}): Hono {
  const router = new Hono();
  const fileBundler = new FileBundler();

  router.get("/api/context", (c) => {
    const slices = options.getContext().get();
    return c.json({ data: slices.map(webContextSliceFromDomain) });
  });

  router.post("/api/context/load", async (c) => {
    const body = await c.req.json();
    const parsed = WebLoadContextCommandSchema.safeParse(body);

    if (!parsed.success) {
      return c.json({ title: "Validation Failed", detail: parsed.error.message }, 400);
    }

    const filesToLoad: Record<string, FileId[]> = {};
    for (const [loaderId, fileIds] of Object.entries(parsed.data.filesToLoad)) {
      filesToLoad[loaderId] = fileIds.map((id) => ({ value: id }));
    }

    await options.loadContext().load(filesToLoad);
    return new Response(null, { status: 204 });
  });

  router.post("/api/context/link", async (c) => {
    const body = await c.req.json();
    const parsed = WebLinkContextCommandSchema.safeParse(body);

    if (!parsed.success) {
      return c.json({ title: "Validation Failed", detail: parsed.error.message }, 400);
    }

    await options.linkContext().link(parsed.data.linkerIds);
    return new Response(null, { status: 204 });
  });

  router.post("/api/context/clear", (c) => {
    options.clearContext().clear();
    return new Response(null, { status: 204 });
  });

  router.get("/api/context/graph-nodes", (c) => {
    const query = c.req.query("query") ?? "";
    const offset = parseInt(c.req.query("offset") ?? "0", 10);
    const limit = parseInt(c.req.query("limit") ?? "10", 10);

    const result = options.getContextGraph().searchNodes(query, offset, limit);
    return c.json({
      nodes: result.nodes.map(webContextGraphNodeFromDomain),
      edges: result.edges.map(webContextGraphEdgeFromDomain),
    });
  });

  router.get("/api/context/graph-nodes/:nodeId/neighbors", (c) => {
    const nodeId = c.req.param("nodeId");
    const result = options.getContextGraph().getNeighbors(nodeId);
    return c.json({
      nodes: result.nodes.map(webContextGraphNodeFromDomain),
      edges: result.edges.map(webContextGraphEdgeFromDomain),
    });
  });

  router.post("/api/context/generate-classes", async (c) => {
    const body = await c.req.json();
    const parsed = WebGenerateClassesRequestSchema.safeParse(body);

    if (!parsed.success) {
      return c.json({ title: "Validation Failed", detail: parsed.error.message }, 400);
    }

    const files = await options.generateClasses().generateClasses(parsed.data.languages ?? []);
    const bundle = fileBundler.writeToBuffer(files);

    return new Response(bundle, {
      status: 200,
      headers: {
        "Content-Type": "application/octet-stream",
        "Content-Disposition": 'attachment; filename="classes.bin"',
      },
    });
  });

  return router;
}
