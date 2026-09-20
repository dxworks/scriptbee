import type { FileId } from "../domain/project.js";

export interface LoadContextUseCase {
  load(filesToLoad: Record<string, FileId[]>): Promise<void>;
}
