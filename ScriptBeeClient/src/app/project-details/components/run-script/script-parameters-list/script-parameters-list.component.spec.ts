import { ScriptParametersListComponent } from './script-parameters-list.component';
import { createComponentFactory } from '@ngneat/spectator';
import { ParameterType } from '../../../services/script-types';
import { MockComponents } from 'ng-mocks';
import { ScriptParameterComponent } from './script-parameter/script-parameter.component';

describe('ScriptParametersListComponent', () => {
  const createComponent = createComponentFactory({
    component: ScriptParametersListComponent,
    imports: [MockComponents(ScriptParameterComponent)],
  });

  it('should create', () => {
    const component = createComponent();

    expect(component).toBeTruthy();
  });

  it('should add parameter', () => {
    const component = createComponent();
    jest.spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();

    expect(component.component.parameters.length).toEqual(1);
    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([
      {
        id: '0',
        name: '',
        type: ParameterType.string,
        nameError: 'Parameter name is required',
      },
    ]);
  });

  it('should delete parameter', () => {
    const component = createComponent();
    jest.spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();
    component.component.onParameterDelete(component.component.parameters[0]);

    expect(component.component.parameters.length).toEqual(0);
    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([]);
  });

  it('should change parameter', () => {
    const component = createComponent();
    jest.spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();
    component.component.onParameterChange({
      id: '0',
      name: 'test',
      type: ParameterType.boolean,
    });

    expect(component.component.parameters.length).toEqual(1);
    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([
      {
        id: '0',
        name: 'test',
        type: ParameterType.boolean,
      },
    ]);
  });

  it('should create unique parameter id even when parameter is deleted', () => {
    const component = createComponent();

    component.component.onAddButtonClick();
    component.component.onParameterDelete(component.component.parameters[0]);
    component.component.onAddButtonClick();

    expect(component.component.parameters[0].id).toEqual('1');
  });

  it('should emit error when parameter name is not unique', () => {
    const component = createComponent();
    jest.spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();
    component.component.onParameterChange({
      id: '0',
      name: 'test',
      type: ParameterType.boolean,
    });
    component.component.onAddButtonClick();
    component.component.onParameterChange({
      id: '1',
      name: 'test',
      type: ParameterType.boolean,
    });
    component.detectComponentChanges();

    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([
      {
        id: '0',
        name: 'test',
        type: ParameterType.boolean,
      },
      {
        id: '1',
        name: 'test',
        type: ParameterType.boolean,
        nameError: 'Parameter name must be unique',
      },
    ]);
  });

  it('should not emit error when parameter name is unique', () => {
    const component = createComponent();
    jest.spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();
    component.component.onParameterChange({
      id: '0',
      name: 'test',
      type: ParameterType.boolean,
    });
    component.component.onAddButtonClick();
    component.component.onParameterChange({
      id: '1',
      name: 'test2',
      type: ParameterType.boolean,
    });
    component.detectComponentChanges();

    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([
      {
        id: '0',
        name: 'test',
        type: ParameterType.boolean,
        nameError: undefined,
      },
      {
        id: '1',
        name: 'test2',
        type: ParameterType.boolean,
        nameError: undefined,
      },
    ]);
  });
});
