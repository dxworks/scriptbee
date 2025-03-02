import { Component } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatTooltip } from '@angular/material/tooltip';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  imports: [MatIconButton, MatTooltip, MatIcon],
})
export class ScriptTreeComponent {
  onCreateNewScriptButtonClick() {
    //   this.dialog.open(CreateScriptDialogComponent, {
    //     disableClose: true,
    //     data: { projectId: this.projectId },
    //   });
  }
}
