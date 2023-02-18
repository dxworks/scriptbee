import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CreateScriptDialogData } from './create-script-dialog-data';
import { ScriptTypes } from './script-types';
import { CreateScriptStore } from '../../../stores/create-script.store';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-create-script-dialog',
  templateUrl: './create-script-dialog.component.html',
  styleUrls: ['./create-script-dialog.component.scss'],
  providers: [CreateScriptStore],
})
export class CreateScriptDialogComponent {
  scriptExists = false;
  types = [ScriptTypes.csharp, ScriptTypes.javascript, ScriptTypes.python];

  availableScriptLanguages$ = this.store.availableLanguages.pipe(map((languages) => languages.map((language) => language.name)));
  availableScriptLanguagesError$ = this.store.availableLanguagesError;

  scriptPath = '';
  scriptType: ScriptTypes = ScriptTypes.csharp;
  loading = false;
  // TODO: remove the store
  // TODO: handle 500 vs 404 errors
  // TODO: add parameters
  // TODO: add tests
  // TODO: call the API

  constructor(
    public dialogRef: MatDialogRef<CreateScriptDialogComponent>,
    private store: CreateScriptStore,
    @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData
  ) {
    this.store.loadAvailableLanguages();
    // this.store.select(selectScriptCreationLoading).subscribe({
    //   next: ({ loading, error }) => {
    //     if (!loading && this.loading) {
    //       if (error) {
    //         this.scriptExists = true;
    //       } else {
    //         this.dialogRef.close();
    //       }
    //     }
    //
    //     this.loading = loading;
    //   },
    // });
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    // this.store.dispatch(
    //   createScript({
    //     projectId: this.data.projectId,
    //     scriptPath: this.scriptPath,
    //     scriptType: this.scriptType,
    //   })
    // );
  }
}
