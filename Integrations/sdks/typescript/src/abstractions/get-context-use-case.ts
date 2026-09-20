import type { ContextSlice } from "../domain/context.js";

export interface GetContextUseCase {
  get(): ContextSlice[];
}
