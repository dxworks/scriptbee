import { Component, output } from '@angular/core';
import { MatTreeModule } from '@angular/material/tree';

import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-tree-actions-menu',
  templateUrl: './tree-actions-menu.component.html',
  imports: [MatTreeModule, MatCheckboxModule, MatButtonModule, MatIconModule, MatMenuModule],
  styleUrls: ['./tree-actions-menu.scss'],
})
export class TreeActionsMenuComponent {
  onDelete = output();
}
