import { z } from "zod";

export const SampleCodeFileSchema = z.object({
  name: z.string().min(1),
  content: z.string(),
});
export type SampleCodeFile = z.infer<typeof SampleCodeFileSchema>;
