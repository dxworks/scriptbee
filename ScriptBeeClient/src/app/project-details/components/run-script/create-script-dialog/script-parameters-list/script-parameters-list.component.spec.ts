import { ScriptParametersListComponent } from './script-parameters-list.component';
import { createComponentFactory } from '@ngneat/spectator';
import { ParameterType } from '../script-types';

describe('ScriptParametersListComponent', () => {
  const createComponent = createComponentFactory({
    component: ScriptParametersListComponent,
  });

  it('should create', () => {
    const component = createComponent();

    expect(component).toBeTruthy();
  });

  it('should add parameter', () => {
    const component = createComponent();
    spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();

    expect(component.component.parameters.length).toEqual(1);
    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([
      {
        id: '0',
        name: '',
        type: ParameterType.string,
      },
    ]);
  });

  it('should delete parameter', () => {
    const component = createComponent();
    spyOn(component.component.parametersChange, 'emit');

    component.component.onAddButtonClick();
    component.component.onParameterDelete(component.component.parameters[0]);

    expect(component.component.parameters.length).toEqual(0);
    expect(component.component.parametersChange.emit).toHaveBeenCalledWith([]);
  });

  it('should change parameter', () => {
    const component = createComponent();
    spyOn(component.component.parametersChange, 'emit');

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
});
