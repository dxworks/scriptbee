import { Component, computed, inject, input, output } from '@angular/core';
import { ErrorResponse } from '../../types/api';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

interface ValidationError {
  name: string;
  messages: string[];
}

@Component({
  selector: 'app-error-state',
  imports: [MatCardModule, MatExpansionModule, MatButtonModule, MatIconModule, MatDividerModule, MatListModule, MatSnackBarModule, MatTooltipModule],
  templateUrl: './error-state.component.html',
  styleUrl: './error-state.component.scss',
})
export class ErrorStateComponent {
  error = input.required<ErrorResponse>();

  retry = output<void>();

  validationErrors = computed<ValidationError[]>(() => {
    const errors = this.error().errors ?? {};

    return Object.keys(errors).map((key) => {
      return {
        name: key,
        messages: errors[key],
      };
    });
  });

  private clipboard = inject(Clipboard);
  private snackBar = inject(MatSnackBar);

  onRetryButtonClick() {
    this.retry.emit();
  }

  onCopySupportInfo() {
    const info = JSON.stringify(this.error(), null, 2);
    this.clipboard.copy(info);
    this.snackBar.open('Support information copied to clipboard', 'Close', {
      duration: 3000,
    });
  }
}
