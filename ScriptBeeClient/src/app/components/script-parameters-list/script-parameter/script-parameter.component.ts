import { Component, computed, input, output, signal } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { Parameter, ParameterType, ParameterValue } from '../../../types/script-types';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { ScriptParameterValueComponent } from './script-parameter-value/script-parameter-value.component';

class ParameterNameErrorMatcher implements ErrorStateMatcher {
  private isError: boolean = false;

  setErrorState(isError: boolean) {
    this.isError = isError;
  }

  isErrorState(): boolean {
    return this.isError;
  }
}

export type ParameterWithError = Parameter & {
  index: number;
  error?: string;
};

@Component({
  selector: 'app-script-parameter',
  templateUrl: './script-parameter.component.html',
  styleUrls: ['./script-parameter.component.scss'],
  imports: [MatSelectModule, MatButtonModule, MatIconModule, MatInput, FormsModule, ScriptParameterValueComponent],
})
export class ScriptParameterComponent {
  parameter = input.required<ParameterWithError>();

  parameterChange = output<ParameterWithError>();
  delete = output<ParameterWithError>();

  parameterTypes = signal<ParameterType[]>(['string', 'float', 'integer', 'boolean']);

  parameterNameErrorMatcher = computed(() => {
    const matcher = new ParameterNameErrorMatcher();
    matcher.setErrorState(this.parameter().error !== undefined);
    return matcher;
  });

  onDeleteClick() {
    this.delete.emit(this.parameter());
  }

  onParameterTypeChange(type: ParameterType) {
    this.parameterChange.emit({ ...this.parameter(), type, value: undefined });
  }

  onParameterNameChange(name: string) {
    this.parameterChange.emit({ ...this.parameter(), name });
  }

  onParameterValueChange(value: ParameterValue) {
    this.parameterChange.emit({ ...this.parameter(), value });
  }
}
