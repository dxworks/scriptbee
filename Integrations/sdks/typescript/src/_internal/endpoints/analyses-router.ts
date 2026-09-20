import { Hono } from "hono";
import type { RunAnalysisUseCase } from "../../abstractions/run-analysis-use-case.js";
import {
  WebRunAnalysisCommandSchema,
  webRunAnalysisResponseFromInfo,
} from "./contracts/web-models.js";

export function makeAnalysesRouter(
  getRunAnalysis: () => RunAnalysisUseCase,
): Hono {
  const router = new Hono();

  router.post("/api/analyses", async (c) => {
    const body = await c.req.json();
    const parsed = WebRunAnalysisCommandSchema.safeParse(body);

    if (!parsed.success) {
      return c.json(
        { title: "Validation Failed", detail: parsed.error.message },
        400,
      );
    }

    const command = {
      projectId: { value: parsed.data.projectId },
      scriptId: { value: parsed.data.scriptId },
    };

    const info = await getRunAnalysis().run(command);
    c.header("Location", `/api/analyses/${info.id.value}`);
    return c.json(webRunAnalysisResponseFromInfo(info), 202);
  });

  return router;
}
