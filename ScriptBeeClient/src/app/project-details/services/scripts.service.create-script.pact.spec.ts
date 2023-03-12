import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { eachLike, like } from '@pact-foundation/pact/src/v3/matchers';
import { term } from '@pact-foundation/pact/src/dsl/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';
import { CreateScriptData, ParameterType } from './script-types';

describe('Create Script Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  it('create script is possible', () => {
    provider
      .given('valid data to create a script')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', 'script path', 'script language'),
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
      executeSuccessfulInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script language')), (script) => {
        expect(script.id).toStrictEqual('script id');
        expect(script.projectId).toStrictEqual('project id');
        expect(script.name).toStrictEqual('script name');
        expect(script.filePath).toStrictEqual('script path');
        expect(script.absolutePath).toStrictEqual('script absolutePath path');
        expect(script.scriptLanguage).toStrictEqual('script language');
        expect(script.parameters[0].name).toStrictEqual('parameter name');
        expect(script.parameters[0].type).toStrictEqual(ParameterType.string);
        expect(script.parameters[0].value).toStrictEqual('parameter value');
      })
    );
  });

  it('empty project id', () => {
    provider
      .given('create script with empty project id')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('', 'script path', 'script language'),
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('', 'script path', 'script language')), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('empty script path', () => {
    provider
      .given('create script with empty script path')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', '', 'script language'),
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('project id', '', 'script language')), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('empty script language', () => {
    provider
      .given('create script with empty script language')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', 'script path', ''),
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', '')), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('invalid script language', () => {
    provider
      .given('create script with invalid script language')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', 'script path', 'invalid script language'),
      })
      .willRespondWith({
        status: 400,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'invalid script language')), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('project id for project that does not exist', () => {
    provider
      .given('create script with project id for project that does not exist')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', 'script path', 'script language'),
      })
      .willRespondWith({
        status: 404,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script language')), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('script path already exists', () => {
    provider
      .given('create script with script path that already exists')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', 'script path', 'script language'),
      })
      .willRespondWith({
        status: 409,
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script language')), (error) => {
        expect(error.status).toStrictEqual(409);
      })
    );
  });

  it('error while creating script', () => {
    provider
      .given('error while creating script')
      .uponReceiving('a request to create a script')
      .withRequest({
        method: 'POST',
        path: '/api/scripts',
        body: createRequestBody('project id', 'script path', 'script language'),
      })
      .willRespondWith({
        status: 500,
        headers: getResponseHeaders(),
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script language')), (error) => {
        expect(error.status).toStrictEqual(500);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });
});

function createRequestBody(projectId: string, filePath: string, scriptType: string) {
  return {
    projectId: like(projectId),
    filePath: like(filePath),
    scriptLanguage: like(scriptType),
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

function createRequestData(projectId: string, filePath: string, scriptType: string): CreateScriptData {
  return {
    projectId: projectId,
    filePath: filePath,
    scriptLanguage: scriptType,
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
