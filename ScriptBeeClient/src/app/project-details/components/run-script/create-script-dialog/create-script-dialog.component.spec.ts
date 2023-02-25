import { CreateScriptDialogComponent } from './create-script-dialog.component';
import { createComponentFactory, createHttpFactory, HttpMethod, Spectator, SpectatorHttp } from '@ngneat/spectator/jest';
import { ScriptsService } from '../../../services/scripts.service';
import { MockComponents, MockDirectives } from 'ng-mocks';
import { ScriptParametersListComponent } from '../script-parameters-list/script-parameters-list.component';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import {
  clickElementById,
  clickElementByText,
  enterTextInInput,
  queryElementByCss,
  queryElementById,
  queryElementByText,
  selectItemInSelect,
} from '../../../../../../test/inputUtils';
import { CreateScriptData, CreateScriptResponse, ParameterType, ScriptLanguage } from '../../../services/script-types';
import { MatSelectModule } from '@angular/material/select';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ApiErrorMessage } from '../../../../shared/api-error-message';
import { waitForAsync } from '@angular/core/testing';
import { CenteredSpinnerComponent } from '../../../../shared/centered-spinner/centered-spinner.component';

describe('CreateScriptDialogComponent', () => {
  let createScriptServiceSpectator: SpectatorHttp<ScriptsService>;
  const createScriptService = createHttpFactory({ service: ScriptsService });
  const mockDialogRef = {
    close: jest.fn(),
  };

  const createComponent = createComponentFactory({
    component: CreateScriptDialogComponent,
    declarations: [...MockComponents(ScriptParametersListComponent, CenteredSpinnerComponent), ...MockDirectives(MatLabel)],
    imports: [MatDialogModule, MatSelectModule, MatExpansionModule, MatFormFieldModule, MatInputModule, FormsModule, ReactiveFormsModule],
    providers: [
      { provide: MAT_DIALOG_DATA, useValue: {} },
      { provide: MatDialogRef, useValue: mockDialogRef },
    ],
  });

  function mockGetAvailableScriptLanguages(component: Spectator<CreateScriptDialogComponent>, languages: ScriptLanguage[]) {
    createScriptServiceSpectator.expectOne('/api/scripts/languages', HttpMethod.GET).flush(languages);

    component.fixture.detectChanges();
  }

  function mockCreateScriptWithoutFlush(component: Spectator<CreateScriptDialogComponent>, request: CreateScriptData) {
    const testRequest = createScriptServiceSpectator.expectOne('/api/scripts', HttpMethod.POST);
    expect(testRequest.request.body).toEqual(request);

    return testRequest;
  }

  function mockCreateScript(component: Spectator<CreateScriptDialogComponent>, request: CreateScriptData, body: CreateScriptResponse | ApiErrorMessage) {
    const testRequest = mockCreateScriptWithoutFlush(component, request);
    const status = 'code' in body ? body.code : 200;

    testRequest.flush(body, { status, statusText: status === 200 ? 'OK' : 'Error' });

    component.fixture.detectChanges();
  }

  beforeEach(() => {
    mockDialogRef.close.mockClear();
    createScriptServiceSpectator = createScriptService();
  });

  it('should create', () => {
    const component = createComponent();

    mockGetAvailableScriptLanguages(component, []);

    expect(component).toBeTruthy();
  });

  it('should contain components', () => {
    const component = createComponent();

    mockGetAvailableScriptLanguages(component, []);

    expect(queryElementByCss(component, 'app-script-parameters-list')).toBeTruthy();
    expect(queryElementById(component, 'script-language')).toBeTruthy();
    expect(queryElementById(component, 'script-name')).toBeTruthy();
    expect(getOkButtonComponent(component)).toBeTruthy();
    expect(getCancelButtonComponent(component)).toBeTruthy();
  });

  it('should load available script languages', () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, [
      {
        name: 'Python',
        extension: 'py',
      },
      {
        name: 'Bash',
        extension: 'sh',
      },
    ]);

    clickElementById(component, 'script-language');

    expect(queryElementByText(component, 'Python')).toBeTruthy();
    expect(queryElementByText(component, 'Bash')).toBeTruthy();
  });

  it('empty values, ok button should be disabled', () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, []);

    expect(getOkButtonComponent(component).nativeElement.disabled).toBeTruthy();
  });

  it('empty language, ok button should be disabled', () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, [{ name: 'Python', extension: 'py' }]);

    enterTextInInput(component, 'script-name', 'test');
    expect(getOkButtonComponent(component).nativeElement.disabled).toBeTruthy();
  });

  it('empty name, ok button should be disabled', async () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, [{ name: 'Python', extension: 'py' }]);

    await selectItemInSelect(component, 0);

    expect(getOkButtonComponent(component).nativeElement.disabled).toBeTruthy();
  });

  it('script name and script type, but empty parameters, ok button should be enabled', async () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);

    enterTextInInput(component, 'script-name', 'test');
    await selectItemInSelect(component, 0);

    expect(component.component.scriptPath).toBe('test');
    expect(component.component.scriptLanguage).toBe('Bash');
    expect(getOkButtonComponent(component).nativeElement.disabled).toBeFalsy();
  });

  it('parameters are not valid, ok button should be disabled', async () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);

    enterTextInInput(component, 'script-name', 'test');
    await selectItemInSelect(component, 0);

    component.component.parameters = [{ id: '0', name: 'test', type: ParameterType.boolean, value: '', nameError: 'error' }];

    component.fixture.detectChanges();

    expect(getOkButtonComponent(component).nativeElement.disabled).toBeTruthy();
  });

  it('should close dialog when cancel button is clicked', () => {
    const component = createComponent();
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);
    jest.spyOn(component.component.dialogRef, 'close');

    getCancelButtonComponent(component).nativeElement.click();

    expect(component.component.dialogRef.close).toHaveBeenCalledTimes(1);
  });

  it('should create script without parameters', async () => {
    const component = createComponent({
      props: {
        data: {
          projectId: 'project-id',
        },
      },
    });
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);

    await selectValidInputs(component);

    mockCreateScript(
      component,
      { projectId: 'project-id', filePath: 'test', scriptLanguage: 'Bash', parameters: [] },
      {
        name: 'test',
        filePath: 'test',
        srcPath: 'test',
        id: 'script-id',
        projectId: 'project-id',
        parameters: [],
        scriptLanguage: 'Bash',
      }
    );
    expect(component.component.dialogRef.close).toHaveBeenCalledTimes(1);
  });

  it('should create script with parameters', async () => {
    const component = createComponent({
      props: {
        data: {
          projectId: 'project-id',
        },
      },
    });
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);

    component.component.parameters = [
      { id: '0', name: 'test', type: ParameterType.boolean, value: 'false', nameError: '' },
      { id: '1', name: 'test2', type: ParameterType.boolean, value: 'true', nameError: '' },
    ];
    await selectValidInputs(component);

    mockCreateScript(
      component,
      {
        projectId: 'project-id',
        filePath: 'test',
        scriptLanguage: 'Bash',
        parameters: [
          { name: 'test', type: ParameterType.boolean, value: 'false' },
          { name: 'test2', type: ParameterType.boolean, value: 'true' },
        ],
      },
      {
        name: 'test',
        filePath: 'test',
        srcPath: 'test',
        id: 'script-id',
        projectId: 'project-id',
        parameters: [
          { name: 'test', type: ParameterType.boolean, value: 'false' },
          { name: 'test2', type: ParameterType.boolean, value: 'true' },
        ],
        scriptLanguage: 'Bash',
      }
    );

    expect(component.component.dialogRef.close).toHaveBeenCalledTimes(1);
  });

  it('should show error when script creation fails', async () => {
    const component = createComponent({
      props: {
        data: {
          projectId: 'project-id',
        },
      },
    });
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);
    await selectValidInputs(component);
    mockCreateScript(
      component,
      { projectId: 'project-id', filePath: 'test', scriptLanguage: 'Bash', parameters: [] },
      {
        code: 500,
        message: 'error',
      }
    );

    expect(queryElementByText(component, 'An error occurred while creating the script')).toBeTruthy();
  });

  it('should show error when script already exists', async () => {
    const component = createComponent({
      props: {
        data: {
          projectId: 'project-id',
        },
      },
    });
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);
    await selectValidInputs(component);
    mockCreateScript(
      component,
      { projectId: 'project-id', filePath: 'test', scriptLanguage: 'Bash', parameters: [] },
      {
        code: 409,
        message: 'error',
      }
    );

    expect(queryElementByText(component, 'A script with this name already exists')).toBeTruthy();
  });

  it('should show loading element while request is still in progress', async () => {
    const component = createComponent({
      props: {
        data: {
          projectId: 'project-id',
        },
      },
    });
    mockGetAvailableScriptLanguages(component, [{ name: 'Bash', extension: 'sh' }]);

    await selectValidInputs(component);
    const request = mockCreateScriptWithoutFlush(component, {
      projectId: 'project-id',
      filePath: 'test',
      scriptLanguage: 'Bash',
      parameters: [],
    });
    component.detectChanges();
    component.fixture.detectChanges();

    await waitForAsync(() => {
      expect(queryElementById(component, 'loading-spinner')).toBeTruthy();
    });
    request.flush({});
  });
});

function getOkButtonComponent(component: Spectator<CreateScriptDialogComponent>) {
  return queryElementByText(component, 'OK');
}

function getCancelButtonComponent(component: Spectator<CreateScriptDialogComponent>) {
  return queryElementByText(component, 'CANCEL');
}

async function selectValidInputs(component: Spectator<CreateScriptDialogComponent>) {
  await selectItemInSelect(component, 0);
  enterTextInInput(component, 'script-name', 'test');
  clickElementByText(component, 'OK');
}
