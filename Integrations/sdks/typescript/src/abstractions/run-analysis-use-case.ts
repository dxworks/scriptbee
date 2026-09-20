import type { ProjectId, ScriptId } from "../domain/project.js";
import type { AnalysisInfo } from "../domain/analysis.js";

export interface RunAnalysisCommand {
  readonly projectId: ProjectId;
  readonly scriptId: ScriptId;
}

export interface RunAnalysisUseCase {
  run(command: RunAnalysisCommand): Promise<AnalysisInfo>;
}
