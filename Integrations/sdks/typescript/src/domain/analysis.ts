import { z } from "zod";
import { InstanceIdSchema, ProjectIdSchema, ScriptIdSchema } from "./project.js";

export const AnalysisIdSchema = z.object({ value: z.string() });
export type AnalysisId = z.infer<typeof AnalysisIdSchema>;

export const AnalysisStatusSchema = z.object({ value: z.string() });
export type AnalysisStatus = z.infer<typeof AnalysisStatusSchema>;

export const ResultIdSchema = z.object({ value: z.string() });
export type ResultId = z.infer<typeof ResultIdSchema>;

export const ResultType = {
  FILE: "File",
  CONSOLE: "Console",
  RUN_ERROR: "RunError",
} as const;
export type ResultType = (typeof ResultType)[keyof typeof ResultType];

export const ResultSummarySchema = z.object({
  id: ResultIdSchema,
  name: z.string(),
  type: z.string(),
  creationDate: z.coerce.date(),
});
export type ResultSummary = z.infer<typeof ResultSummarySchema>;

export const AnalysisInfoSchema = z.object({
  id: AnalysisIdSchema,
  projectId: ProjectIdSchema,
  instanceId: InstanceIdSchema,
  scriptId: ScriptIdSchema,
  status: AnalysisStatusSchema,
  creationDate: z.coerce.date(),
  results: z.array(ResultSummarySchema).default([]),
  errors: z.array(z.string()).default([]),
  finishedDate: z.coerce.date().nullable().optional(),
});
export type AnalysisInfo = z.infer<typeof AnalysisInfoSchema>;

export function isRunning(info: AnalysisInfo): boolean {
  return info.status.value === "Started" || info.status.value === "Running";
}
