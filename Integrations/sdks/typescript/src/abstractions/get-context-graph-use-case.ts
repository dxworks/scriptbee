import type { ContextGraphResult } from "../domain/context.js";

export interface GetContextGraphUseCase {
  searchNodes(query: string, offset: number, limit: number): ContextGraphResult;
  getNeighbors(nodeId: string): ContextGraphResult;
}
