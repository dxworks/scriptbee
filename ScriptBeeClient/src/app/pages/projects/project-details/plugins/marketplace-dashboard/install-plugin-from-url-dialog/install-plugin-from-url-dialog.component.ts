import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-install-plugin-from-url-dialog',
  templateUrl: './install-plugin-from-url-dialog.component.html',
  styleUrls: ['./install-plugin-from-url-dialog.component.scss'],
  imports: [MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, FormsModule],
})
export class InstallPluginFromUrlDialogComponent {
  url = signal('');

  private dialogRef = inject(MatDialogRef<InstallPluginFromUrlDialogComponent>);

  confirm() {
    const trimmed = this.url().trim();
    if (trimmed) {
      this.dialogRef.close(trimmed);
    }
  }
}
