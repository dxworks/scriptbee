import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-install-plugin-from-url-dialog',
  template: `
    <h2 mat-dialog-title>Install Plugin from URL</h2>
    <mat-dialog-content>
      <p class="dialog-description">
        Provide a direct URL to a ZIP file. The archive must contain a
        <code>manifest.yaml</code> in its root folder.
      </p>
      <mat-form-field appearance="outline" class="url-field">
        <mat-label>Plugin ZIP URL</mat-label>
        <mat-icon matPrefix>link</mat-icon>
        <input
          matInput
          type="url"
          placeholder="https://example.com/plugin.zip"
          [(ngModel)]="url"
          (keyup.enter)="confirm()"
        />
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary" [disabled]="!url().trim()" (click)="confirm()">Install</button>
    </mat-dialog-actions>
  `,
  styles: [
    `
      .dialog-description {
        margin-bottom: 16px;
        opacity: 0.8;
      }
      .url-field {
        width: 100%;
      }
      code {
        font-family: monospace;
        background: var(--mat-sys-surface-variant);
        padding: 2px 4px;
        border-radius: 4px;
      }
    `,
  ],
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
