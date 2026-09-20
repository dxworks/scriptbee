import { describe, expect, it } from "vitest";
import { ResultType, type ResultSummary } from "../../src/domain/analysis.js";
import { DefaultAnalysisResultService } from "../../src/results/analysis-result-service.js";
import type { ScriptResultsStore } from "../../src/results/script-results-store.js";

class MockScriptResultsStore implements ScriptResultsStore {
  readonly uploadedFiles = new Map<string, { content: Uint8Array; metadata?: Record<string, string> | undefined }>();

  async uploadFile(fileId: string, content: Uint8Array, metadata?: Record<string, string>): Promise<void> {
    this.uploadedFiles.set(fileId, { content, metadata });
  }

  getFile(fileId: string): Uint8Array | undefined {
    return this.uploadedFiles.get(fileId)?.content;
  }

  hasFile(fileId: string): boolean {
    return this.uploadedFiles.has(fileId);
  }
}

describe("DefaultAnalysisResultService", () => {
  it("addFile with string content stores utf-8 bytes and fires callback", async () => {
    const store = new MockScriptResultsStore();
    const fixedDate = new Date("2026-01-01T12:00:00Z");
    const recorded: ResultSummary[] = [];

    const service = new DefaultAnalysisResultService({
      store,
      idGenerator: () => "custom-id-1",
      dateProvider: () => fixedDate,
      onResultAdded: (s) => recorded.push(s),
    });

    const resultId = await service.addFile("output.txt", "file content");

    expect(resultId.value).toBe("custom-id-1");
    expect(store.getFile("custom-id-1")).toEqual(new TextEncoder().encode("file content"));
    expect(recorded).toHaveLength(1);
    expect(recorded[0]?.id.value).toBe("custom-id-1");
    expect(recorded[0]?.name).toBe("output.txt");
    expect(recorded[0]?.type).toBe(ResultType.FILE);
    expect(recorded[0]?.creationDate).toBe(fixedDate);
  });

  it("addFile with Uint8Array content stores bytes as-is", async () => {
    const store = new MockScriptResultsStore();
    const service = new DefaultAnalysisResultService({
      store,
      idGenerator: () => "bytes-id",
    });

    const bytes = new Uint8Array([0, 1, 2, 3]);
    await service.addFile("binary.bin", bytes);

    expect(store.getFile("bytes-id")).toEqual(bytes);
  });

  it("addConsole stores content and fires callback with CONSOLE type", async () => {
    const store = new MockScriptResultsStore();
    const recorded: ResultSummary[] = [];

    const service = new DefaultAnalysisResultService({
      store,
      idGenerator: () => "console-id",
      onResultAdded: (s) => recorded.push(s),
    });

    const resultId = await service.addConsole("log line");

    expect(resultId.value).toBe("console-id");
    expect(store.getFile("console-id")).toEqual(new TextEncoder().encode("log line"));
    expect(recorded[0]?.name).toBe("ConsoleOutput");
    expect(recorded[0]?.type).toBe(ResultType.CONSOLE);
  });

  it("addError stores message and fires callback with RUN_ERROR type", async () => {
    const store = new MockScriptResultsStore();
    const recorded: ResultSummary[] = [];

    const service = new DefaultAnalysisResultService({
      store,
      idGenerator: () => "error-id",
      onResultAdded: (s) => recorded.push(s),
    });

    await service.addError("Something went wrong");

    expect(store.getFile("error-id")).toEqual(new TextEncoder().encode("Something went wrong"));
    expect(recorded[0]?.name).toBe("RunError");
    expect(recorded[0]?.type).toBe(ResultType.RUN_ERROR);
  });

  it("addResult with custom type fires callback with that type", async () => {
    const store = new MockScriptResultsStore();
    const recorded: ResultSummary[] = [];

    const service = new DefaultAnalysisResultService({
      store,
      idGenerator: () => "custom-type-id",
      onResultAdded: (s) => recorded.push(s),
    });

    await service.addResult("CustomChart", "Chart", "chart-data");

    expect(store.getFile("custom-type-id")).toEqual(new TextEncoder().encode("chart-data"));
    expect(recorded[0]?.name).toBe("CustomChart");
    expect(recorded[0]?.type).toBe("Chart");
  });

  it("uses real uuid and date when no overrides are provided", async () => {
    const store = new MockScriptResultsStore();
    const service = new DefaultAnalysisResultService({ store });

    const resultId = await service.addFile("default.txt", "test");

    expect(resultId.value).not.toBe("");
    expect(store.hasFile(resultId.value)).toBe(true);
  });

  it("does not fire callback when no onResultAdded is given", async () => {
    const store = new MockScriptResultsStore();
    const service = new DefaultAnalysisResultService({ store, idGenerator: () => "no-cb" });
    await expect(service.addFile("f.txt", "x")).resolves.toEqual({ value: "no-cb" });
  });
});
