import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Parameter, ParameterType, ParameterTypeValues } from '../../script-types';
import { distinctUntilChanged, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-script-parameter',
  templateUrl: './script-parameter.component.html',
  styleUrls: ['./script-parameter.component.scss'],
})
export class ScriptParameterComponent {
  @Input()
  parameter: Parameter;

  @Output()
  parameterChange = new EventEmitter<Parameter>();

  @Output()
  delete = new EventEmitter<Parameter>();

  parameterTypes = ParameterTypeValues;
  nameUpdate = new Subject<string>();

  constructor() {
    this.nameUpdate.pipe(debounceTime(400), distinctUntilChanged()).subscribe((name) => this.onParameterNameChange(name));
  }

  onDeleteClick() {
    this.delete.emit(this.parameter);
  }

  onParameterTypeChange(type: ParameterType) {
    this.parameterChange.emit({ ...this.parameter, type });
  }

  onParameterNameChange(name: string) {
    this.parameterChange.emit({ ...this.parameter, name });
  }
}
