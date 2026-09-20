import { z } from "zod";

export const ContextSliceSchema = z.object({
  model: z.string(),
  pluginIds: z.array(z.string()),
});
export type ContextSlice = z.infer<typeof ContextSliceSchema>;

export const ContextGraphNodeSchema = z.object({
  id: z.string(),
  label: z.string(),
  type: z.string(),
  loader: z.string().nullable().optional(),
  properties: z.record(z.unknown()).default({}),
});
export type ContextGraphNode = z.infer<typeof ContextGraphNodeSchema>;

export const ContextGraphEdgeSchema = z.object({
  source: z.string(),
  target: z.string(),
  label: z.string(),
});
export type ContextGraphEdge = z.infer<typeof ContextGraphEdgeSchema>;

export const ContextGraphResultSchema = z.object({
  nodes: z.array(ContextGraphNodeSchema),
  edges: z.array(ContextGraphEdgeSchema),
});
export type ContextGraphResult = z.infer<typeof ContextGraphResultSchema>;
