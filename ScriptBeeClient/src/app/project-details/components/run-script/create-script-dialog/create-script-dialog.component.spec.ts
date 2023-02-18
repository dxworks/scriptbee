import { CreateScriptDialogComponent } from './create-script-dialog.component';
import { createComponentFactory } from '@ngneat/spectator';

describe('CreateScriptDialogComponent', () => {
  const createComponent = createComponentFactory({
    component: CreateScriptDialogComponent,
  });

  it('should create', () => {
    const component = createComponent();

    expect(component).toBeTruthy();
  });
});
