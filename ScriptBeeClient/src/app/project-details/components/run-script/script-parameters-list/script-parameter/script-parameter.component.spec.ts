import { ScriptParameterComponent } from './script-parameter.component';
import { createComponentFactory } from '@ngneat/spectator/jest';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ParameterType } from '../../../../services/script-types';
import { waitForAsync } from '@angular/core/testing';
import { clickElementByText, clickSelectOption, enterTextInInput, queryElementByText } from '../../../../../../../test/inputUtils';
import { MatCheckboxModule } from '@angular/material/checkbox';

describe('ScriptParameterComponent', () => {
  const createComponent = createComponentFactory({
    component: ScriptParameterComponent,
    imports: [MatSelectModule, MatInputModule, MatIconModule, MatCheckboxModule, ReactiveFormsModule, FormsModule],
  });

  it('should create', () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });

    expect(component).toBeTruthy();
  });

  it('should contain a delete button', () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });

    const deleteButton = queryElementByText<ScriptParameterComponent>(component, 'delete');

    expect(deleteButton).toBeTruthy();
  });

  it('given plugin, then plugin name and type are visible', () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });

    const nameElement = queryElementByText(component, 'Parameter Name');
    const typeElement = queryElementByText(component, 'Parameter Type');

    expect(nameElement).toBeTruthy();
    expect(typeElement).toBeTruthy();
  });

  it('parameter error is present, then error message is visible', () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
          nameError: 'An error is present',
        },
      },
    });

    const errorMessage = queryElementByText(component, 'An error is present');
    expect(errorMessage).toBeTruthy();
  });

  it('given parameter type string, then value input of type text', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });

    const valueInput = queryElementByText(component, 'Parameter Value');

    await waitForAsync(() => {
      expect(valueInput.nativeElement.type).toBe('text');
    });
  });

  it('given parameter type integer, then value input of type number without decimals', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.integer,
        },
      },
    });

    const valueInput = queryElementByText(component, 'Parameter Value');

    await waitForAsync(() => {
      expect(valueInput.nativeElement.type).toBe('number');
      expect(valueInput.nativeElement.step).toBe('1');
    });
  });

  it('given parameter type float, then value input of type number with decimals', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.float,
        },
      },
    });

    const valueInput = queryElementByText(component, 'Parameter Value');

    await waitForAsync(() => {
      expect(valueInput).toBeDefined();
      expect(valueInput.nativeElement.type).toBe('number');
    });
  });

  it('given parameter type boolean, then value input of type checkbox', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.boolean,
        },
      },
    });

    const valueInput = queryElementByText(component, 'Parameter Value');

    await waitForAsync(() => {
      expect(valueInput.nativeElement.type).toBe('checkbox');
    });
  });

  it('given new plugin name, then name is updated', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });
    jest.spyOn(component.component.parameterChange, 'emit');

    enterTextInInput(component, 'parameter-name', 'new name');

    await waitForAsync(() => {
      expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
      expect(component.component.parameterChange.emit).toHaveBeenCalledWith({ ...component.component.parameter, name: 'new name' });
    });
  });

  it('given new plugin type, then type is updated', () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });
    jest.spyOn(component.component.parameterChange, 'emit');

    clickSelectOption(component, 'parameter-type', 'boolean');

    expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
    expect(component.component.parameterChange.emit).toHaveBeenCalledWith({
      ...component.component.parameter,
      type: ParameterType.boolean,
    });
  });

  it('given string value, then value is updated', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });
    jest.spyOn(component.component.parameterChange, 'emit');

    await waitForAsync(() => {
      enterTextInInput(component, 'parameter-value', 'value');
    });

    await waitForAsync(() => {
      expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
      expect(component.component.parameterChange.emit).toHaveBeenCalledWith({
        ...component.component.parameter,
        value: 'value',
      });
    });
  });

  it('given integer value, then value is updated', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.integer,
        },
      },
    });
    jest.spyOn(component.component.parameterChange, 'emit');

    await waitForAsync(() => {
      enterTextInInput(component, 'parameter-value', '1');
    });

    await waitForAsync(() => {
      expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
      expect(component.component.parameterChange.emit).toHaveBeenCalledWith({
        ...component.component.parameter,
        value: 1,
      });
    });
  });

  it('given float value, then value is updated', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.float,
        },
      },
    });
    jest.spyOn(component.component.parameterChange, 'emit');

    await waitForAsync(() => {
      enterTextInInput(component, 'parameter-value', '1.1');
    });

    await waitForAsync(() => {
      expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
      expect(component.component.parameterChange.emit).toHaveBeenCalledWith({
        ...component.component.parameter,
        value: 1.1,
      });
    });
  });

  it('given boolean value, then value is updated', async () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.boolean,
        },
      },
    });
    jest.spyOn(component.component.parameterChange, 'emit');

    await waitForAsync(() => {
      clickElementByText(component, 'Parameter Value');
    });

    await waitForAsync(() => {
      expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
      expect(component.component.parameterChange.emit).toHaveBeenCalledWith({
        ...component.component.parameter,
        value: 'true',
      });
    });
  });

  it('given delete button click, then delete event is emitted', () => {
    const component = createComponent({
      props: {
        parameter: {
          id: '1',
          name: 'name',
          type: ParameterType.string,
        },
      },
    });
    jest.spyOn(component.component.delete, 'emit');

    clickElementByText(component, 'delete');

    expect(component.component.delete.emit).toHaveBeenCalledTimes(1);
    expect(component.component.delete.emit).toHaveBeenCalledWith(component.component.parameter);
  });
});
