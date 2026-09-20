import type { SampleCodeFile } from "../domain/code-generation.js";

export interface GenerateClassesUseCase {
  generateClasses(languages: string[]): Promise<SampleCodeFile[]>;
}
