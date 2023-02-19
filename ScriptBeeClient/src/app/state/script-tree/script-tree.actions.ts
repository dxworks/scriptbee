import { createAction, props } from '@ngrx/store';
import { FileTreeNode } from '../../project-details/components/run-script/scripts-content/fileTreeNode';
import { ScriptTreeNode } from './script-tree.state';
import { ScriptTypes } from '../../project-details/services/script-types';

export const fetchScriptTree = createAction('[ScriptBee] Fetch Script Tree', props<{ projectId: string }>());

export const fetchScriptTreeSuccess = createAction('[ScriptBee] Fetch Script Tree Success', props<{ tree: FileTreeNode[] }>());

export const fetchScriptTreeFailure = createAction('[ScriptBee] Fetch Script Tree Failure', props<{ error: string }>());

export const scriptTreeLeafClick = createAction('[ScriptBee] Script Tree Leaf Click', props<{ node: ScriptTreeNode }>());

export const createScript = createAction('[ScriptBee] Create Script', props<{ projectId: string; scriptPath: string; scriptType: ScriptTypes }>());

export const createScriptSuccess = createAction('[ScriptBee] Create Script Success', props<{ node: FileTreeNode }>());

export const createScriptFailure = createAction('[ScriptBee] Create Script Failure', props<{ error: string }>());
