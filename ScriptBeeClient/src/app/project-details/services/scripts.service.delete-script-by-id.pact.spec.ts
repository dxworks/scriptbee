import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { like } from '@pact-foundation/pact/src/v3/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';

describe('Delete Script by Id Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  it('delete script by id possible', () => {
    provider
      .given('valid script id and project id')
      .uponReceiving('a request to delete script by id')
      .withRequest({
        method: 'DELETE',
        path: '/api/scripts/script-id',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 200,
      });

    return provider.executeTest(() => executeSuccessfulInteraction(scriptService.deleteScript('script-id', 'project-id')));
  });

  it('empty project id', () => {
    provider
      .given('empty project id for delete script by id')
      .uponReceiving('a request to delete script by id')
      .withRequest({
        method: 'DELETE',
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
      executeFailedInteraction(scriptService.deleteScript('script-id', ''), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('script id for project that does not exist', () => {
    provider
      .given('script id for project that does not exist for delete script by id')
      .uponReceiving('a request to delete script by id')
      .withRequest({
        method: 'DELETE',
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
      executeFailedInteraction(scriptService.deleteScript('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('project id for project that does not exist', () => {
    provider
      .given('project id for project that does not exist for delete script by id')
      .uponReceiving('a request to delete script by id')
      .withRequest({
        method: 'DELETE',
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
      executeFailedInteraction(scriptService.deleteScript('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('error while deleting script by id', () => {
    provider
      .given('error while deleting script by id')
      .uponReceiving('a request to delete script by id')
      .withRequest({
        method: 'DELETE',
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
      executeFailedInteraction(scriptService.deleteScript('script-id', 'project-id'), (error) => {
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
