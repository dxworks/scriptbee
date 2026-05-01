export interface ContextGraphNode {
  id: string;
  label: string;
  type: string;
  loader?: string;
  properties: Record<string, unknown>;
}

export interface ContextGraphEdge {
  source: string;
  target: string;
  label: string;
}

export interface ContextGraphResult {
  nodes: ContextGraphNode[];
  edges: ContextGraphEdge[];
}
