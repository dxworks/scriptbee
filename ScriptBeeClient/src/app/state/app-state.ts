import { OutputState } from './outputs/output.state';
import { ScriptTreeState } from './script-tree/script-tree.state';

export interface AppState {
  scriptTree: ScriptTreeState;
  outputState: OutputState;
}
