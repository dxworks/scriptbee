import { Component, EventEmitter, Output } from '@angular/core';
import { MatExpansionPanel } from '@angular/material/expansion';
import { Parameter, ParameterType } from '../../../services/script-types';

@Component({
  selector: 'app-script-parameters-list',
  templateUrl: './script-parameters-list.component.html',
  styleUrls: ['./script-parameters-list.component.scss'],
  viewProviders: [MatExpansionPanel],
})
export class ScriptParametersListComponent {
  @Output()
  parametersChange = new EventEmitter<Parameter[]>();

  parameters: Parameter[] = [];
  private parameterId = 0;

  onAddButtonClick(): void {
    this.parameters.push(
      this.setParameterError({
        id: this.parameterId.toString(),
        name: '',
        type: ParameterType.string,
      })
    );
    this.parameterId++;

    this.parametersChange.emit(this.parameters);
  }

  onParameterDelete(parameter: Parameter): void {
    this.parameters = this.parameters.filter((p) => p.id !== parameter.id);

    this.parametersChange.emit(this.parameters);
  }

  onParameterChange(parameter: Parameter) {
    this.parameters = this.parameters.map((p) => (p.id === parameter.id ? this.setParameterError(parameter) : p));

    this.parametersChange.emit(this.parameters);
  }

  setParameterError(parameter: Parameter): Parameter {
    const isParameterNameUnique = this.parameters.every((p) => p.id === parameter.id || p.name !== parameter.name);
    let nameError = undefined;

    if (!parameter.name) {
      nameError = 'Parameter name is required';
    } else if (!isParameterNameUnique) {
      nameError = 'Parameter name must be unique';
    }

    return {
      ...parameter,
      nameError,
    };
  }
}
