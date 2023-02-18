import { CreateScriptDialogComponent } from './create-script-dialog.component';
import { createComponentFactory, createHttpFactory, HttpMethod, Spectator, SpectatorHttp } from '@ngneat/spectator';
import { ScriptsService } from '../../../services/scripts.service';
import { MockComponents } from 'ng-mocks';
import { ScriptParametersListComponent } from './script-parameters-list/script-parameters-list.component';
import { MatFormField } from '@angular/material/form-field';
import { ScriptLanguage } from '../../../services/create-script';

describe('CreateScriptDialogComponent', () => {
  let createScriptServiceSpectator: SpectatorHttp<ScriptsService>;
  const createScriptService = createHttpFactory({ service: ScriptsService });

  const createComponent = createComponentFactory({
    component: CreateScriptDialogComponent,
    declarations: [...MockComponents(ScriptParametersListComponent, MatFormField)],
  });

  function mockGetAvailableScriptLanguages(component: Spectator<CreateScriptDialogComponent>, languages: ScriptLanguage[]) {
    createScriptServiceSpectator.expectOne('/api/scripts/languages', HttpMethod.GET).flush(languages);

    component.fixture.detectChanges();
  }

  beforeEach(() => {
    createScriptServiceSpectator = createScriptService();
  });

  it('should create', () => {
    const component = createComponent();

    mockGetAvailableScriptLanguages(component, []);

    expect(component).toBeTruthy();
  });
});
