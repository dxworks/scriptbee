import { Component, input, output } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { ScriptParameter, ParameterValue } from '../../../../types/script-types';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';

@Component({
  selector: 'app-script-parameter-value',
  templateUrl: './script-parameter-value.component.html',
  styleUrls: ['./script-parameter-value.component.scss'],
  imports: [MatSelectModule, MatButtonModule, MatIconModule, MatInput, FormsModule, MatCheckbox],
})
export class ScriptParameterValueComponent {
  parameter = input.required<ScriptParameter>();

  valueChange = output<ParameterValue>();

  onParameterValueChange(value: ParameterValue) {
    this.valueChange.emit(value);
  }

  isCheckedValue(value: ParameterValue | undefined) {
    if (value === undefined) {
      return false;
    }

    return !!value;
  }
}
