import { ApiErrorMessageComponent } from './api-error-message.component';
import { createComponentFactory } from '@ngneat/spectator';
import { MockComponent } from 'ng-mocks';
import { MatIcon } from '@angular/material/icon';
import { By } from '@angular/platform-browser';

describe('ApiErrorMessageComponent', () => {
  const createComponent = createComponentFactory({
    component: ApiErrorMessageComponent,
    declarations: [MockComponent(MatIcon)]
  });

  it('should create', () => {
    const component = createComponent();
    expect(component).toBeTruthy();
  });

  it('given type is no-data, then no-data message is shown', () => {
    const component = createComponent({ props: { type: 'no-data', message: 'No data' } });

    const icon = component.debugElement.query(By.directive(MatIcon));

    expect(icon.componentInstance.fontIcon).toEqual('assignment_late');
    expect(component.debugElement.nativeElement.textContent).toContain('No data');
  });

  it('given type is error, then error message is shown', () => {
    const component = createComponent({ props: { type: 'error', message: 'Error' } });

    const icon = component.debugElement.query(By.directive(MatIcon));

    expect(icon.componentInstance.fontIcon).toEqual('report');
    expect(component.debugElement.nativeElement.textContent).toContain('Error');
  });

  it('given type is warning, then warning message is shown', () => {
    const component = createComponent({ props: { type: 'warning', message: 'Warning' } });

    const icon = component.debugElement.query(By.directive(MatIcon));

    expect(icon.componentInstance.fontIcon).toEqual('warning');
    expect(component.debugElement.nativeElement.textContent).toContain('Warning');
  });
});
