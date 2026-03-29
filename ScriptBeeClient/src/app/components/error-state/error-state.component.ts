import { Component, computed, input, output } from '@angular/core';
import { ErrorResponse } from '../../types/api';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

interface ValidationError {
  name: string;
  messages: string[];
}

@Component({
  selector: 'app-error-state',
  imports: [MatCardModule, MatExpansionModule, MatButtonModule, MatIconModule],
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

  onRetryButtonClick() {
    this.retry.emit();
  }
}
