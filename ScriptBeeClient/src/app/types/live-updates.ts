export interface BaseScriptEvent {
  projectId: string;
  scriptId: string;
  clientId: string;
}

export type ScriptUpdatedEvent = BaseScriptEvent;
export type ScriptDeletedEvent = BaseScriptEvent;
export interface ScriptCreateEvent extends BaseScriptEvent {
  parentId: string | null;
  path: string;
}

export interface AnalysisStatusChangedEvent {
  projectId: string;
  analysisId: string;
  status: string;
}
