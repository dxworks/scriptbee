import { z } from "zod";
import type { AnalysisInfo } from "../../../domain/analysis.js";
import type { ContextGraphEdge, ContextGraphNode, ContextSlice } from "../../../domain/context.js";
import type { Plugin } from "../../../domain/plugins.js";

export const WebRunAnalysisCommandSchema = z.object({
  projectId: z.string().min(1),
  scriptId: z.string().min(1),
});
export type WebRunAnalysisCommand = z.infer<typeof WebRunAnalysisCommandSchema>;

export interface WebRunAnalysisResponse {
  id: string;
  projectId: string;
  scriptId: string;
  status: string;
  creationDate: string;
}

export function webRunAnalysisResponseFromInfo(info: AnalysisInfo): WebRunAnalysisResponse {
  return {
    id: info.id.value,
    projectId: info.projectId.value,
    scriptId: info.scriptId.value,
    status: info.status.value,
    creationDate: info.creationDate.toISOString(),
  };
}

export const WebLoadContextCommandSchema = z.object({
  filesToLoad: z.record(z.array(z.string())),
});
export type WebLoadContextCommand = z.infer<typeof WebLoadContextCommandSchema>;

export const WebLinkContextCommandSchema = z.object({
  linkerIds: z.array(z.string()),
});
export type WebLinkContextCommand = z.infer<typeof WebLinkContextCommandSchema>;

export interface WebContextSlice {
  model: string;
  pluginIds: string[];
}

export function webContextSliceFromDomain(cs: ContextSlice): WebContextSlice {
  return { model: cs.model, pluginIds: cs.pluginIds };
}

export interface WebGetContextResponse {
  data: WebContextSlice[];
}

export const WebGenerateClassesRequestSchema = z.object({
  languages: z.array(z.string()).optional(),
  transferFormat: z.string().optional(),
});
export type WebGenerateClassesRequest = z.infer<typeof WebGenerateClassesRequestSchema>;

export interface WebContextGraphNode {
  id: string;
  label: string;
  type: string;
  loader?: string | null | undefined;
  properties: Record<string, unknown>;
}

export function webContextGraphNodeFromDomain(node: ContextGraphNode): WebContextGraphNode {
  return {
    id: node.id,
    label: node.label,
    type: node.type,
    loader: node.loader,
    properties: node.properties,
  };
}

export interface WebContextGraphEdge {
  source: string;
  target: string;
  label: string;
}

export function webContextGraphEdgeFromDomain(edge: ContextGraphEdge): WebContextGraphEdge {
  return { source: edge.source, target: edge.target, label: edge.label };
}

export interface WebContextGraphResponse {
  nodes: WebContextGraphNode[];
  edges: WebContextGraphEdge[];
}

export const WebInstallPluginCommandSchema = z.object({
  pluginId: z.string().min(1),
  version: z.string().min(1),
});
export type WebInstallPluginCommand = z.infer<typeof WebInstallPluginCommandSchema>;

export interface WebInstalledPluginManifest {
  apiVersion: string;
  name: string;
  description?: string | null | undefined;
  author?: string | null | undefined;
  extensionPoints: unknown[];
}

export interface WebInstalledPlugin {
  folderPath: string;
  id: string;
  version: string;
  manifest: WebInstalledPluginManifest;
}

export function webInstalledPluginFromDomain(plugin: Plugin): WebInstalledPlugin {
  return {
    folderPath: plugin.folderPath,
    id: plugin.id.name,
    version: plugin.id.version,
    manifest: {
      apiVersion: plugin.manifest.apiVersion,
      name: plugin.manifest.name,
      description: plugin.manifest.description,
      author: plugin.manifest.author,
      extensionPoints: plugin.manifest.extensionPoints,
    },
  };
}

export interface WebGetInstalledPluginsResponse {
  data: WebInstalledPlugin[];
}
