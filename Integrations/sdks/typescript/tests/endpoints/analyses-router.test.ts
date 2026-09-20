import { describe, expect, it, vi } from "vitest";
import type { RunAnalysisUseCase } from "../../src/abstractions/run-analysis-use-case.js";
import { makeAnalysesRouter } from "../../src/_internal/endpoints/analyses-router.js";

describe("makeAnalysesRouter", () => {
  it("returns 202 and Location header for valid run analysis command", async () => {
    const fixedDate = new Date("2026-01-01T00:00:00.000Z");
    const mockRunAnalysis: RunAnalysisUseCase = {
      run: vi.fn().mockResolvedValue({
        id: { value: "analysis-123" },
        projectId: { value: "proj-1" },
        scriptId: { value: "script-1" },
        status: { value: "Running" },
        creationDate: fixedDate,
      }),
    };

    const router = makeAnalysesRouter(() => mockRunAnalysis);

    const response = await router.request("/api/analyses", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        projectId: "proj-1",
        scriptId: "script-1",
      }),
    });

    expect(response.status).toBe(202);
    expect(response.headers.get("Location")).toBe("/api/analyses/analysis-123");

    const json = await response.json();
    expect(json).toEqual({
      id: "analysis-123",
      projectId: "proj-1",
      scriptId: "script-1",
      status: "Running",
      creationDate: "2026-01-01T00:00:00.000Z",
    });
    expect(mockRunAnalysis.run).toHaveBeenCalledWith({
      projectId: { value: "proj-1" },
      scriptId: { value: "script-1" },
    });
  });

  it("returns 400 when body fails schema validation", async () => {
    const mockRunAnalysis: RunAnalysisUseCase = {
      run: vi.fn(),
    };

    const router = makeAnalysesRouter(() => mockRunAnalysis);

    const response = await router.request("/api/analyses", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        projectId: "",
      }),
    });

    expect(response.status).toBe(400);
    const json = (await response.json()) as { title: string };
    expect(json.title).toBe("Validation Failed");
    expect(mockRunAnalysis.run).not.toHaveBeenCalled();
  });
});
