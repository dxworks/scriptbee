export interface BaseScriptEvent {
  projectId: string;
  scriptId: string;
  clientId: string;
}

export type ScriptUpdatedEvent = BaseScriptEvent;
export type ScriptDeletedEvent = BaseScriptEvent;
export type ScriptCreateEvent = BaseScriptEvent;

export interface AnalysisStatusChangedEvent {
  projectId: string;
  analysisId: string;
  status: string;
}
