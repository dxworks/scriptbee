import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CreateScriptDialogData } from './create-script-dialog-data';
import { ScriptTypes } from './script-types';
import { Store } from '@ngrx/store';
import { createScript } from '../../../../state/script-tree/script-tree.actions';
import { selectScriptCreationLoading } from '../../../../state/script-tree/script-tree.selectors';

@Component({
  selector: 'app-create-script-dialog',
  templateUrl: './create-script-dialog.component.html',
  styleUrls: ['./create-script-dialog.component.scss'],
})
export class CreateScriptDialogComponent {
  scriptExists = false;
  types = [ScriptTypes.csharp, ScriptTypes.javascript, ScriptTypes.python];

  scriptPath = '';
  scriptType: ScriptTypes = ScriptTypes.csharp;
  loading = false;
  // TODO: remove the store
  // TODO: handle 500 vs 404 errors
  // TODO: add parameters
  // TODO: add tests
  // TODO: call the API

  constructor(public dialogRef: MatDialogRef<CreateScriptDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData, private store: Store) {
    this.store.select(selectScriptCreationLoading).subscribe({
      next: ({ loading, error }) => {
        if (!loading && this.loading) {
          if (error) {
            this.scriptExists = true;
          } else {
            this.dialogRef.close();
          }
        }

        this.loading = loading;
      },
    });
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    this.store.dispatch(
      createScript({
        projectId: this.data.projectId,
        scriptPath: this.scriptPath,
        scriptType: this.scriptType,
      })
    );
  }
}
