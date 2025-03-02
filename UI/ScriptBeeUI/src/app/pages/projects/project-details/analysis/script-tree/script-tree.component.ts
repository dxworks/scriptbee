import { Component, input, output, signal } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatTooltip } from '@angular/material/tooltip';
import { MatIcon } from '@angular/material/icon';
import { SelectableTreeComponent } from '../../../../../components/selectable-tree/selectable-tree.component';
import { TreeNode } from '../../../../../types/tree-node';

@Component({
  selector: 'app-script-tree',
  templateUrl: './script-tree.component.html',
  styleUrls: ['./script-tree.component.scss'],
  imports: [MatIconButton, MatTooltip, MatIcon, SelectableTreeComponent],
})
export class ScriptTreeComponent {
  projectId = input.required<string>();

  onFileSelected = output<TreeNode>();

  fileTreeNodes = signal<TreeNode[]>([
    {
      name: 'folder1',
      children: [{ name: 'sub-folder-1', children: [{ name: 'file' }] }],
    },
    {
      name: 'folder-2',
      children: [
        {
          name: 'sub-folder-1',
          children: [{ name: 'file' }],
        },
        { name: 'file-2' },
      ],
    },
  ]);

  onCreateNewScriptButtonClick() {
    //   this.dialog.open(CreateScriptDialogComponent, {
    //     disableClose: true,
    //     data: { projectId: this.projectId },
    //   });
  }

  onNodeDelete(node: TreeNode) {
    // TODO FIXIT: implement it
    console.log('delete node', node);
  }

  onNodeClick(node: TreeNode) {
    this.onFileSelected.emit(node);
  }
}
