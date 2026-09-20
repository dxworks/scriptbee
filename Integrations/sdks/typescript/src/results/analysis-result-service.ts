import { randomUUID } from "node:crypto";
import { ResultId, ResultSummary, ResultType } from "../domain/analysis.js";
import type { ScriptResultsStore } from "./script-results-store.js";

export interface AnalysisResultService {
  addFile(
    name: string,
    content: string | Uint8Array,
    resultType?: string,
  ): Promise<ResultId>;
  addConsole(content: string, name?: string): Promise<ResultId>;
  addError(message: string, name?: string): Promise<ResultId>;
  addResult(
    name: string,
    resultType: string,
    content: string | Uint8Array,
  ): Promise<ResultId>;
}

export class DefaultAnalysisResultService implements AnalysisResultService {
  private readonly store: ScriptResultsStore;
  private readonly idGenerator: () => string;
  private readonly dateProvider: () => Date;
  private readonly onResultAdded:
    ((summary: ResultSummary) => void) | undefined;

  constructor(options: {
    store: ScriptResultsStore;
    idGenerator?: () => string;
    dateProvider?: () => Date;
    onResultAdded?: (summary: ResultSummary) => void;
  }) {
    this.store = options.store;
    this.idGenerator = options.idGenerator ?? (() => randomUUID());
    this.dateProvider = options.dateProvider ?? (() => new Date());
    this.onResultAdded = options.onResultAdded;
  }

  async addFile(
    name: string,
    content: string | Uint8Array,
    resultType: string = ResultType.FILE,
  ): Promise<ResultId> {
    return this.addResult(name, resultType, content);
  }

  async addConsole(content: string, name = "ConsoleOutput"): Promise<ResultId> {
    return this.addResult(name, ResultType.CONSOLE, content);
  }

  async addError(message: string, name = "RunError"): Promise<ResultId> {
    return this.addResult(name, ResultType.RUN_ERROR, message);
  }

  async addResult(
    name: string,
    resultType: string,
    content: string | Uint8Array,
  ): Promise<ResultId> {
    const rawBytes =
      typeof content === "string" ? new TextEncoder().encode(content) : content;

    const resultId: ResultId = { value: this.idGenerator() };
    await this.store.uploadFile(resultId.value, rawBytes);

    if (this.onResultAdded !== undefined) {
      const summary: ResultSummary = {
        id: resultId,
        name,
        type: resultType,
        creationDate: this.dateProvider(),
      };
      this.onResultAdded(summary);
    }

    return resultId;
  }
}
