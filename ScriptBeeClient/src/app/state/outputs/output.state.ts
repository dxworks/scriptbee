export interface OutputState {
  runIndex?: number;
  scriptName?: string;
  results: Result[];
}

export interface Result {
  id: string;
  name: string;
  type: string;
}
