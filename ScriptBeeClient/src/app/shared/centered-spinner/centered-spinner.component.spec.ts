import { CenteredSpinnerComponent } from './centered-spinner.component';
import { createComponentFactory } from '@ngneat/spectator/jest';
import { MockComponent } from 'ng-mocks';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { queryElementById } from '../../../../test/inputUtils';

describe('CenteredSpinnerComponent', () => {
  const createComponent = createComponentFactory({
    component: CenteredSpinnerComponent,
    declarations: [MockComponent(MatProgressSpinner)],
  });

  it('should create', () => {
    const component = createComponent();

    expect(component).toBeTruthy();
  });

  it('should be visible by default', () => {
    const component = createComponent();

    expect(queryElementById(component, 'loading-spinner')).toBeTruthy();
  });

  it('should be invisible', () => {
    const component = createComponent({
      props: {
        visible: false,
      },
    });

    expect(queryElementById(component, 'loading-spinner')).toBeFalsy();
  });
});
