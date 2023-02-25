import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { eachLike, like } from '@pact-foundation/pact/src/v3/matchers';
import { term } from '@pact-foundation/pact/src/dsl/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';
import { ParameterType } from './script-types';

describe('Get Script by Id Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  it('get script by id possible', () => {
    provider
      .given('valid script id and project id')
      .uponReceiving('a request to get script by id')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id',
        query: {
          projectId: like('project-id'),
        },
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
      executeSuccessfulInteraction(scriptService.getScriptById('script-id', 'project-id'), (script) => {
        expect(script.id).toStrictEqual('script id');
        expect(script.projectId).toStrictEqual('project id');
        expect(script.scriptLanguage).toStrictEqual('script language');
        expect(script.absolutePath).toStrictEqual('script absolutePath path');
        expect(script.filePath).toStrictEqual('script path');
        expect(script.name).toStrictEqual('script name');
        expect(script.parameters[0].name).toStrictEqual('parameter name');
        expect(script.parameters[0].type).toStrictEqual(ParameterType.string);
        expect(script.parameters[0].value).toStrictEqual('parameter value');
      })
    );
  });

  it('empty project id', () => {
    provider
      .given('empty project id for get script by id')
      .uponReceiving('a request to get script by id')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id',
        query: {
          projectId: '',
        },
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptById('script-id', ''), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('script id for project that does not exist', () => {
    provider
      .given('script id for project that does not exist for get script by id')
      .uponReceiving('a request to get script by id')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 404,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptById('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('project id for project that does not exist', () => {
    provider
      .given('project id for project that does not exist for get script by id')
      .uponReceiving('a request to get script by id')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 404,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptById('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('error while getting script by id', () => {
    provider
      .given('error while getting script by id')
      .uponReceiving('a request to get script by id')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 500,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptById('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(500);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });
});

function createErrorResponse() {
  return {
    message: like('message'),
  };
}
