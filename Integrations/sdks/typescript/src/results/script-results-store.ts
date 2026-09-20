export interface ScriptResultsStore {
  uploadFile(fileId: string, content: Uint8Array, metadata?: Record<string, string>): Promise<void>;
}
