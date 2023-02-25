import { EditParametersDialogComponent } from './edit-parameters-dialog.component';
import { ScriptsService } from '../../../services/scripts.service';
import { createComponentFactory, createHttpFactory, HttpMethod, Spectator, SpectatorHttp } from '@ngneat/spectator/jest';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MockComponents } from 'ng-mocks';
import { ScriptParametersListComponent } from '../script-parameters-list/script-parameters-list.component';
import { CenteredSpinnerComponent } from '../../../../shared/centered-spinner/centered-spinner.component';
import { queryElementByCss, queryElementById, queryElementByText } from '../../../../../../test/inputUtils';
import { ParameterType, UpdateScriptData, UpdateScriptResponse } from '../../../services/script-types';
import { ApiErrorMessage } from '../../../../shared/api-error-message';
import { waitForAsync } from '@angular/core/testing';

describe('EditParametersDialogComponent', () => {
  let updateScriptServiceSpectator: SpectatorHttp<ScriptsService>;
  const createScriptService = createHttpFactory({ service: ScriptsService });
  const mockDialogRef = {
    close: jest.fn(),
  };

  const createComponent = createComponentFactory({
    component: EditParametersDialogComponent,
    declarations: [...MockComponents(ScriptParametersListComponent, CenteredSpinnerComponent)],
    imports: [MatDialogModule],
    providers: [
      { provide: MAT_DIALOG_DATA, useValue: { parameters: [] } },
      { provide: MatDialogRef, useValue: mockDialogRef },
    ],
  });

  function mockUpdateScript(component: Spectator<EditParametersDialogComponent>, request: UpdateScriptData, body: UpdateScriptResponse | ApiErrorMessage) {
    const testRequest = mockUpdateScriptWithoutFlush(component, request);
    const status = 'code' in body ? body.code : 200;

    testRequest.flush(body, { status, statusText: status === 200 ? 'OK' : 'Error' });

    component.fixture.detectChanges();
  }

  function mockUpdateScriptWithoutFlush(component: Spectator<EditParametersDialogComponent>, request: UpdateScriptData) {
    const testRequest = updateScriptServiceSpectator.expectOne('/api/scripts', HttpMethod.PUT);
    expect(testRequest.request.body).toEqual(request);

    return testRequest;
  }

  beforeEach(() => {
    mockDialogRef.close.mockClear();

    updateScriptServiceSpectator = createScriptService();
  });

  it('should create', () => {
    const component = createComponent({
      props: {
        data: {
          scriptId: 'scriptId',
          projectId: 'projectId',
          parameters: [],
        },
      },
    });

    expect(component).toBeTruthy();
  });

  it('should contain components', () => {
    const component = createComponent({
      props: {
        data: {
          scriptId: 'scriptId',
          projectId: 'projectId',
          parameters: [],
        },
      },
    });

    expect(queryElementByCss(component, 'app-script-parameters-list')).toBeTruthy();
    expect(getUpdateButtonComponent(component)).toBeTruthy();
    expect(getCancelButtonComponent(component)).toBeTruthy();
  });

  it('should close dialog on cancel click', () => {
    const component = createComponent({
      props: {
        data: {
          scriptId: 'scriptId',
          projectId: 'projectId',
          parameters: [],
        },
      },
    });

    getCancelButtonComponent(component).nativeElement.click();

    expect(mockDialogRef.close).toHaveBeenCalled();
  });

  it('should update script parameters on update click', () => {
    const component = createComponent({
      props: {
        data: {
          scriptId: 'scriptId',
          projectId: 'projectId',
          parameters: [
            {
              id: 'id',
              name: 'name',
              type: ParameterType.boolean,
              value: 'value',
            },
          ],
        },
      },
    });
    component.detectChanges();
    component.fixture.detectChanges();
    getUpdateButtonComponent(component).nativeElement.click();

    mockUpdateScript(
      component,
      {
        id: 'scriptId',
        projectId: 'projectId',
        parameters: [
          {
            name: 'name',
            type: ParameterType.boolean,
            value: 'value',
          },
        ],
      },
      {
        id: 'scriptId',
        projectId: 'projectId',
        parameters: [
          {
            name: 'name',
            type: ParameterType.boolean,
            value: 'value',
          },
        ],
        filePath: 'filePath',
        srcPath: 'srcPath',
        scriptLanguage: 'scriptLanguage',
        name: 'filePath',
      }
    );

    expect(component.component.dialogRef.close).toHaveBeenCalledTimes(1);
  });

  it('should show error message on update error', () => {
    const component = createComponent({
      props: {
        data: {
          scriptId: 'scriptId',
          projectId: 'projectId',
          parameters: [
            {
              id: 'id',
              name: 'name',
              type: ParameterType.boolean,
              value: 'value',
            },
          ],
        },
      },
    });
    component.detectChanges();
    component.fixture.detectChanges();
    getUpdateButtonComponent(component).nativeElement.click();

    mockUpdateScript(
      component,
      {
        id: 'scriptId',
        projectId: 'projectId',
        parameters: [
          {
            name: 'name',
            type: ParameterType.boolean,
            value: 'value',
          },
        ],
      },
      {
        code: 400,
        message: 'message',
      }
    );

    expect(queryElementByText(component, 'An error occurred while updating the parameters')).toBeTruthy();
  });

  it('should show loading element while request is still in progress', async () => {
    const component = createComponent({
      props: {
        data: {
          scriptId: 'scriptId',
          projectId: 'projectId',
          parameters: [
            {
              id: 'id',
              name: 'name',
              type: ParameterType.boolean,
              value: 'value',
            },
          ],
        },
      },
    });
    component.detectChanges();
    component.fixture.detectChanges();
    getUpdateButtonComponent(component).nativeElement.click();

    const request = mockUpdateScriptWithoutFlush(component, {
      id: 'scriptId',
      projectId: 'projectId',
      parameters: [
        {
          name: 'name',
          type: ParameterType.boolean,
          value: 'value',
        },
      ],
    });
    component.detectChanges();
    component.fixture.detectChanges();

    await waitForAsync(() => {
      expect(queryElementById(component, 'loading-spinner')).toBeTruthy();
    });
    request.flush({});
  });
});

function getUpdateButtonComponent(component: Spectator<EditParametersDialogComponent>) {
  return queryElementByText(component, 'UPDATE');
}

function getCancelButtonComponent(component: Spectator<EditParametersDialogComponent>) {
  return queryElementByText(component, 'CANCEL');
}
