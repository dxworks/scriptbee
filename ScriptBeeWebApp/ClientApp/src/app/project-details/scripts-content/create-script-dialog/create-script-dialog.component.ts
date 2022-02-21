import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {CreateScriptDialogData} from './create-script-dialog-data';
import {FileSystemService} from '../../../services/file-system/file-system.service';
import {ScriptTypes} from './script-types';
import {ProjectDetailsService} from '../../project-details.service';

@Component({
  selector: 'app-create-script-dialog',
  templateUrl: './create-script-dialog.component.html',
  styleUrls: ['./create-script-dialog.component.scss']
})
export class CreateScriptDialogComponent {

  scriptExists = false;
  types = [ScriptTypes.csharp, ScriptTypes.javascript, ScriptTypes.python];

  constructor(public dialogRef: MatDialogRef<CreateScriptDialogComponent>, private fileSystemService: FileSystemService,
              @Inject(MAT_DIALOG_DATA) public data: CreateScriptDialogData, private projectDetailsService: ProjectDetailsService) {
    if (!data || !data.scriptPath || !data.scriptType) {
      this.data = {scriptPath: '', scriptType: ScriptTypes.csharp};
    }
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onOkClick(): void {
    this.projectDetailsService.project.subscribe(project => {
      if (project) {
        this.fileSystemService.createScript(project.projectId, this.data.scriptPath, this.data.scriptType).subscribe(res => {
          if (res) {
            this.dialogRef.close(this.data.scriptPath);
          }
        }, (error: any) => {
          this.scriptExists = true;
        });
      }
    });
  }
}
