import { Component, computed, input, output, signal } from '@angular/core';
import { MatExpansionPanel } from '@angular/material/expansion';
import { Parameter } from '../../types/script-types';
import { ParameterWithError, ScriptParameterComponent } from './script-parameter/script-parameter.component';
import { MatButtonModule } from '@angular/material/button';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-script-parameters-list',
  templateUrl: './script-parameters-list.component.html',
  styleUrls: ['./script-parameters-list.component.scss'],
  viewProviders: [MatExpansionPanel],
  imports: [ScriptParameterComponent, MatButtonModule, MatDivider],
})
export class ScriptParametersListComponent {
  parameters = input.required<Parameter[]>();

  parametersChange = output<Parameter[]>();
  hasParameterErrors = output<boolean>();

  parametersWithErrors = computed<ParameterWithError[]>(() => {
    const parameters = this.parameters();
    const updatedParameters = parameters.map((p, index) => ({
      ...p,
      index,
      error: this.getParameterError(p, parameters),
    }));

    this.hasParameterErrors.emit(updatedParameters.some((p) => !!p.error));
    return updatedParameters;
  });

  parameterIndex = signal<number>(1);

  onAddButtonClick(): void {
    const index = this.parameterIndex();
    this.parametersChange.emit([
      ...this.parametersWithErrors(),
      {
        name: 'parameter-' + index,
        type: 'string',
        index,
      },
    ]);
    this.parameterIndex.set(index + 1);
  }

  onParameterDelete(parameter: ParameterWithError): void {
    this.parametersChange.emit(this.parameters().filter((_, index) => index !== parameter.index));
  }

  onParameterChange(parameter: ParameterWithError) {
    this.parametersChange.emit(this.parameters().map((p, index) => (index === parameter.index ? parameter : p)));
  }

  private getParameterError(parameter: Parameter, parameters: Parameter[]): string | undefined {
    if (!parameter.name) {
      return 'Parameter name is required';
    }

    if (!this.isParameterNameUnique(parameter, parameters)) {
      return 'Parameter name must be unique';
    }

    return undefined;
  }

  private isParameterNameUnique(parameter: Parameter, parameters: Parameter[]): boolean {
    let numbersOfApparitions = 0;
    for (let i = 0; i < parameters.length; i++) {
      if (parameters[i].name === parameter.name) {
        numbersOfApparitions++;
      }
    }

    return numbersOfApparitions === 1;
  }
}
