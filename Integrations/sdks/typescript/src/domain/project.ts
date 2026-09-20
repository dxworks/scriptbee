import { z } from "zod";

export const ProjectIdSchema = z.object({ value: z.string().min(1) });
export type ProjectId = z.infer<typeof ProjectIdSchema>;

export const ScriptIdSchema = z.object({ value: z.string().min(1) });
export type ScriptId = z.infer<typeof ScriptIdSchema>;

export const FileIdSchema = z.object({ value: z.string().min(1) });
export type FileId = z.infer<typeof FileIdSchema>;

export const InstanceIdSchema = z.object({ value: z.string() });
export type InstanceId = z.infer<typeof InstanceIdSchema>;
