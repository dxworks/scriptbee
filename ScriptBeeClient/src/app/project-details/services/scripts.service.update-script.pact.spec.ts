import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { eachLike, like } from '@pact-foundation/pact/src/v3/matchers';
import { term } from '@pact-foundation/pact/src/dsl/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';
import { ParameterType, UpdateScriptData } from './script-types';

describe('Update Script Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  it('update script is possible', () => {
    provider
      .given('valid data to update a script')
      .uponReceiving('a request to update a script')
      .withRequest({
        method: 'PUT',
        path: '/api/scripts',
        body: createRequestBody('id', 'project id'),
      })
      .willRespondWith({
        status: 200,
        headers: getResponseHeaders(),
        body: {
          id: like('script id'),
          projectId: like('project id'),
          name: like('script name'),
          filePath: like('script path'),
          absolutePath: like('script absolutePath path'),
          scriptLanguage: like('script language'),
          parameters: eachLike({
            name: like('parameter name'),
            type: term({
              matcher: 'string|integer|float|boolean',
              generate: 'string',
            }),
            value: like('parameter value'),
          }),
        },
      });

    return provider.executeTest(() =>
      executeSuccessfulInteraction(scriptService.updateScript(createRequestData('script id', 'project id')), (script) => {
        expect(script.name).toStrictEqual('script name');
        expect(script.projectId).toStrictEqual('project id');
        expect(script.id).toStrictEqual('script id');
        expect(script.absolutePath).toStrictEqual('script absolutePath path');
        expect(script.scriptLanguage).toStrictEqual('script language');
        expect(script.parameters[0].value).toStrictEqual('parameter value');
        expect(script.parameters[0].name).toStrictEqual('parameter name');
        expect(script.parameters[0].type).toStrictEqual(ParameterType.string);
        expect(script.filePath).toStrictEqual('script path');
      })
    );
  });

  it('empty script id', () => {
    provider
      .given('update script with empty script id')
      .uponReceiving('a request to update a script')
      .withRequest({
        method: 'PUT',
        path: '/api/scripts',
        body: createRequestBody('', 'project id'),
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.updateScript(createRequestData('', 'project id')), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('empty project id', () => {
    provider
      .given('update script with empty project id')
      .uponReceiving('a request to update a script')
      .withRequest({
        method: 'PUT',
        path: '/api/scripts',
        body: createRequestBody('id', ''),
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.updateScript(createRequestData('id', '')), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('script id for project that does not exist', () => {
    provider
      .given('update script for id that does not exist')
      .uponReceiving('a request to update a script')
      .withRequest({
        method: 'PUT',
        path: '/api/scripts',
        body: createRequestBody('id', 'project id'),
      })
      .willRespondWith({
        status: 404,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.updateScript(createRequestData('id', 'project id')), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('project id for project that does not exist', () => {
    provider
      .given('update script with project id for project that does not exist')
      .uponReceiving('a request to update a script')
      .withRequest({
        method: 'PUT',
        path: '/api/scripts',
        body: createRequestBody('id', 'project id'),
      })
      .willRespondWith({
        status: 404,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.updateScript(createRequestData('id', 'project id')), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('error while updating script', () => {
    provider
      .given('error while updating script')
      .uponReceiving('a request to update a script')
      .withRequest({
        method: 'PUT',
        path: '/api/scripts',
        body: createRequestBody('id', 'project id'),
      })
      .willRespondWith({
        status: 500,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.updateScript(createRequestData('id', 'project id')), (error) => {
        expect(error.status).toStrictEqual(500);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });
});

function createRequestBody(id: string, projectId: string) {
  return {
    id: like(id),
    projectId: like(projectId),
    parameters: eachLike({
      name: like('parameter name'),
      type: term({
        matcher: 'string|integer|float|boolean',
        generate: 'string',
      }),
      value: like('parameter value'),
    }),
  };
}

function createRequestData(id: string, projectId: string): UpdateScriptData {
  return {
    id,
    projectId,
    parameters: [
      {
        name: 'parameter name',
        type: ParameterType.string,
        value: 'parameter value',
      },
    ],
  };
}

function createErrorResponse() {
  return {
    message: like('message'),
  };
}
