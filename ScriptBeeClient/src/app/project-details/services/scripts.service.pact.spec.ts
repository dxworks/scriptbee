import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { eachLike, like } from '@pact-foundation/pact/src/v3/matchers';
import { term } from '@pact-foundation/pact/src/dsl/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';
import { ParameterType } from './script-types';

describe('Scripts Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  describe('Script Languages', function () {
    it('script languages exist', () => {
      provider
        .given('existing script languages')
        .uponReceiving('a request for script languages')
        .withRequest({
          method: 'GET',
          path: '/api/scripts/languages',
        })
        .willRespondWith({
          status: 200,
          headers: getResponseHeaders(),
          body: eachLike({
            name: like('JavaScript'),
            extension: like('js'),
          }),
        });

      return provider.executeTest(() =>
        executeSuccessfulInteraction(scriptService.getAvailableLanguages(), (scriptLanguages) => {
          expect(scriptLanguages[0].name).toEqual('JavaScript');
          expect(scriptLanguages[0].extension).toEqual('js');
        })
      );
    });

    it('error while getting script languages', () => {
      provider
        .given('an error while getting script languages')
        .uponReceiving('a request for script languages')
        .withRequest({
          method: 'GET',
          path: '/api/scripts/languages',
        })
        .willRespondWith({
          status: 500,
          headers: getResponseHeaders(),
          body: createErrorResponse(),
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.getAvailableLanguages(), (error) => {
          expect(error.status).toEqual(500);
          expect(error.error.message).toEqual('message');
        })
      );
    });
  });

  describe('Create Script', function () {
    function createRequestBody(projectId: string, filePath: string, scriptType: string) {
      return {
        projectId: like(projectId),
        filePath: like(filePath),
        scriptType: like(scriptType),
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

    function createRequestData(projectId: string, filePath: string, scriptType: string) {
      return {
        projectId: projectId,
        filePath: filePath,
        scriptType: scriptType,
        parameters: [
          {
            name: 'parameter name',
            type: ParameterType.string,
            value: 'parameter value',
          },
        ],
      };
    }

    it('create script is possible', () => {
      provider
        .given('valid data to create a script')
        .uponReceiving('a request to create a script')
        .withRequest({
          method: 'POST',
          path: '/api/scripts',
          body: createRequestBody('project id', 'script path', 'script type'),
        })
        .willRespondWith({
          status: 200,
          headers: getResponseHeaders(),
          body: {
            name: like('script name'),
            filePath: like('script path'),
            srcPath: like('script src path'),
          },
        });

      return provider.executeTest(() =>
        executeSuccessfulInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script type')), (script) => {
          expect(script.name).toEqual('script name');
          expect(script.filePath).toEqual('script path');
          expect(script.srcPath).toEqual('script src path');
          expect(script.children).toBeUndefined();
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
          body: createRequestBody('', 'script path', 'script type'),
        })
        .willRespondWith({
          status: 400,
          headers: getResponseHeaders(),
          body: createErrorResponse(),
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.createScript(createRequestData('', 'script path', 'script type')), (error) => {
          expect(error.status).toEqual(400);
          expect(error.error.message).toEqual('message');
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
          body: createRequestBody('project id', '', 'script type'),
        })
        .willRespondWith({
          status: 400,
          headers: getResponseHeaders(),
          body: createErrorResponse(),
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.createScript(createRequestData('project id', '', 'script type')), (error) => {
          expect(error.status).toEqual(400);
          expect(error.error.message).toEqual('message');
        })
      );
    });

    it('empty script type', () => {
      provider
        .given('create script with empty script type')
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
          expect(error.status).toEqual(400);
          expect(error.error.message).toEqual('message');
        })
      );
    });

    it('invalid script type', () => {
      provider
        .given('create script with invalid script type')
        .uponReceiving('a request to create a script')
        .withRequest({
          method: 'POST',
          path: '/api/scripts',
          body: createRequestBody('project id', 'script path', 'invalid script type'),
        })
        .willRespondWith({
          status: 400,
          headers: getResponseHeaders(),
          body: createErrorResponse(),
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'invalid script type')), (error) => {
          expect(error.status).toEqual(400);
          expect(error.error.message).toEqual('message');
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
          body: createRequestBody('project id', 'script path', 'script type'),
        })
        .willRespondWith({
          status: 404,
          headers: getResponseHeaders(),
          body: createErrorResponse(),
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script type')), (error) => {
          expect(error.status).toEqual(404);
          expect(error.error.message).toEqual('message');
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
          body: createRequestBody('project id', 'script path', 'script type'),
        })
        .willRespondWith({
          status: 409,
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script type')), (error) => {
          expect(error.status).toEqual(409);
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
          body: createRequestBody('project id', 'script path', 'script type'),
        })
        .willRespondWith({
          status: 500,
          headers: getResponseHeaders(),
          body: createErrorResponse(),
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.createScript(createRequestData('project id', 'script path', 'script type')), (error) => {
          expect(error.status).toEqual(500);
          expect(error.error.message).toEqual('message');
        })
      );
    });
  });
});

function createErrorResponse() {
  return {
    message: like('message'),
  };
}
