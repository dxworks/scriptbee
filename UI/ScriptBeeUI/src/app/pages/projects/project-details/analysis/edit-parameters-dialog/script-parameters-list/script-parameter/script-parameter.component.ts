import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Parameter, ParameterType, ParameterTypeValues } from '../../../../services/script-types';
import { distinctUntilChanged, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { AbstractControl, FormGroupDirective, NgForm } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';

class ParameterNameErrorMatcher implements ErrorStateMatcher {
  private isError: boolean;

  setErrorState(isError: boolean) {
    this.isError = isError;
  }

  isErrorState(control: AbstractControl | null, form: FormGroupDirective | NgForm | null): boolean {
    return this.isError;
  }
}

@Component({
  selector: 'app-script-parameter',
  templateUrl: './script-parameter.component.html',
  styleUrls: ['./script-parameter.component.scss'],
})
export class ScriptParameterComponent {
  @Input()
  get parameter(): Parameter {
    return this._parameter;
  }

  set parameter(value: Parameter) {
    this._parameter = value;
    this.parameterNameErrorMatcher.setErrorState(value.nameError !== undefined);
  }

  private _parameter: Parameter;

  @Output()
  parameterChange = new EventEmitter<Parameter>();

  @Output()
  delete = new EventEmitter<Parameter>();

  parameterNameErrorMatcher = new ParameterNameErrorMatcher();

  parameterTypes = ParameterTypeValues;
  nameUpdate = new Subject<string>();
  valueUpdate = new Subject<string>();

  constructor() {
    this.nameUpdate.pipe(debounceTime(400), distinctUntilChanged()).subscribe((name) => this.onParameterNameChange(name));
    this.valueUpdate.pipe(debounceTime(400), distinctUntilChanged()).subscribe((value) => this.onParameterValueChange(value));
  }

  onDeleteClick() {
    this.delete.emit(this._parameter);
  }

  onParameterTypeChange(type: ParameterType) {
    this.parameterChange.emit({ ...this._parameter, type });
  }

  onParameterNameChange(name: string) {
    this.parameterChange.emit({ ...this._parameter, name });
  }

  onParameterValueChange(value: string) {
    this.parameterChange.emit({ ...this._parameter, value });
  }

  isCheckedValue(value: string | undefined) {
    if (value === undefined) {
      return false;
    }

    return value === 'true';
  }

  setCheckedValue(checked: boolean) {
    return checked ? 'true' : 'false';
  }

  onIntegerValueKeyDown($event: KeyboardEvent) {
    if ($event.key === '.') {
      $event.preventDefault();
    }
  }

  onIntegerValueInput($event) {
    $event.target.value = $event.target.value.replace(/[^0-9]*/g, '');
  }
}
