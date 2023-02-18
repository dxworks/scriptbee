import { ScriptParameterComponent } from './script-parameter.component';
import { createComponentFactory } from '@ngneat/spectator';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { ParameterType } from '../../script-types';
import { By } from '@angular/platform-browser';
import { waitForAsync } from '@angular/core/testing';

describe('ScriptParameterComponent', () => {
  const createComponent = createComponentFactory({
    component: ScriptParameterComponent,
    imports: [MatSelectModule, MatInputModule, MatIconModule, FormsModule],
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

    const deleteButton = component.debugElement.query((e) => e.nativeElement.textContent === 'delete');

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

    const nameElement = component.debugElement.query((e) => e.nativeElement.textContent === 'Parameter Name');
    const typeElement = component.debugElement.query((e) => e.nativeElement.textContent === 'Parameter Type');

    expect(nameElement).toBeTruthy();
    expect(typeElement).toBeTruthy();
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
    spyOn(component.component.parameterChange, 'emit');

    const nameElement = component.debugElement.query(By.css('#parameter-name')).nativeElement;

    nameElement.value = 'new name';
    nameElement.dispatchEvent(new Event('input'));

    component.detectChanges();

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
    spyOn(component.component.parameterChange, 'emit');

    const typeElement = component.debugElement.query(By.css('#parameter-type')).nativeElement;

    typeElement.click();
    component.detectChanges();
    const option = component.debugElement.query((e) => e.nativeElement.textContent === 'boolean');
    option.nativeElement.click();
    component.detectChanges();

    expect(component.component.parameterChange.emit).toHaveBeenCalledTimes(1);
    expect(component.component.parameterChange.emit).toHaveBeenCalledWith({
      ...component.component.parameter,
      type: ParameterType.boolean,
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
    spyOn(component.component.delete, 'emit');

    const deleteButton = component.debugElement.query((e) => e.nativeElement.textContent === 'delete');

    deleteButton.nativeElement.click();

    expect(component.component.delete.emit).toHaveBeenCalledTimes(1);
    expect(component.component.delete.emit).toHaveBeenCalledWith(component.component.parameter);
  });
});
