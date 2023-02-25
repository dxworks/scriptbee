import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { boolean, eachLike, like } from '@pact-foundation/pact/src/v3/matchers';
import { term } from '@pact-foundation/pact/src/dsl/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';
import { ParameterType } from './script-types';

describe('Get Scripts Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  it('get scripts possible', () => {
    provider
      .given('valid project id while getting scripts')
      .uponReceiving('a request to get scripts')
      .withRequest({
        method: 'GET',
        path: '/api/scripts',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 200,
        headers: getResponseHeaders(),
        body: getScriptNodesResponsePact(),
      });

    return provider.executeTest(() =>
      executeSuccessfulInteraction(scriptService.getScripts('project-id'), (nodes) => {
        const node = nodes[0];
        const scriptData = node.scriptData;
        expect(node.name).toStrictEqual('name');
        expect(node.path).toStrictEqual('path');
        expect(node.absolutePath).toStrictEqual('absolutePath');
        expect(node.isDirectory).toBeTruthy();
        expect(node.children).toHaveLength(1);
        expect(scriptData.id).toStrictEqual('script id');
        expect(scriptData.name).toStrictEqual('script name');
        expect(scriptData.projectId).toStrictEqual('project id');
        expect(scriptData.scriptLanguage).toStrictEqual('script language');
        expect(scriptData.absolutePath).toStrictEqual('script absolutePath');
        expect(scriptData.filePath).toStrictEqual('script path');
        expect(scriptData.parameters[0].name).toStrictEqual('parameter name');
        expect(scriptData.parameters[0].type).toStrictEqual(ParameterType.string);
        expect(scriptData.parameters[0].value).toStrictEqual('parameter value');
      })
    );
  });

  it('empty project id', () => {
    provider
      .given('empty project id for get scripts')
      .uponReceiving('a request to get scripts')
      .withRequest({
        method: 'GET',
        path: '/api/scripts',
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
      executeFailedInteraction(scriptService.getScripts(''), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('project id for project that does not exist', () => {
    provider
      .given('project id for project that does not exist for get scripts')
      .uponReceiving('a request to get scripts')
      .withRequest({
        method: 'GET',
        path: '/api/scripts',
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
      executeFailedInteraction(scriptService.getScripts('project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('error while getting scripts', () => {
    provider
      .given('error while getting scripts')
      .uponReceiving('a request to get scripts')
      .withRequest({
        method: 'GET',
        path: '/api/scripts',
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
      executeFailedInteraction(scriptService.getScripts('project-id'), (error) => {
        expect(error.status).toStrictEqual(500);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });
});

function getNodeResponsePact() {
  return {
    name: like('name'),
    path: like('path'),
    absolutePath: like('absolutePath'),
    isDirectory: boolean(true),
    scriptData: like({
      id: like('script id'),
      projectId: like('project id'),
      name: like('script name'),
      filePath: like('script path'),
      absolutePath: like('script absolutePath'),
      scriptLanguage: like('script language'),
      parameters: eachLike({
        name: like('parameter name'),
        type: term({
          matcher: 'string|integer|float|boolean',
          generate: 'string',
        }),
        value: like('parameter value'),
      }),
    }),
  };
}

function getNodeResponseWithChildrenPact() {
  return {
    ...getNodeResponsePact(),
    children: eachLike(getNodeResponsePact()),
  };
}

function getScriptNodesResponsePact() {
  return eachLike(getNodeResponseWithChildrenPact());
}

function createErrorResponse() {
  return {
    message: like('message'),
  };
}
