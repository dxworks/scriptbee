import { Component, computed, input } from '@angular/core';
import { ErrorResponse } from '../../types/api';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';

interface ValidationError {
  name: string;
  messages: string[];
}

@Component({
  selector: 'app-error-state',
  imports: [MatCardModule, MatExpansionModule],
  templateUrl: './error-state.component.html',
  styleUrl: './error-state.component.scss',
})
export class ErrorStateComponent {
  error = input.required<ErrorResponse>();

  validationErrors = computed<ValidationError[]>(() => {
    const errors = this.error().errors ?? {};

    return Object.keys(errors).map((key) => {
      return {
        name: key,
        messages: errors[key],
      };
    });
  });
}
