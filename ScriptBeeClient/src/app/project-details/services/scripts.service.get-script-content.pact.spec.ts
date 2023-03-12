import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, pactWrapper } from '../../../../test/pactUtils';
import { like } from '@pact-foundation/pact/src/v3/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';

describe('Get Script Content Service Pact', () => {
  const provider = pactWrapper();
  let scriptService: ScriptsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScriptsService, getPactInterceptor()],
      imports: [HttpClientModule],
    });

    scriptService = TestBed.inject(ScriptsService);
  });

  it('get script content possible', () => {
    provider
      .given('valid script id and project id for get script content')
      .uponReceiving('a request to get script content')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id/content',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 200,
        body: like('script content'),
      });

    return provider.executeTest(() =>
      executeSuccessfulInteraction(scriptService.getScriptContent('script-id', 'project-id'), (content) => {
        expect(content).toStrictEqual('script content');
      })
    );
  });

  it('empty project id', () => {
    provider
      .given('empty project id for get script content')
      .uponReceiving('a request to get script content')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id/content',
        query: {
          projectId: '',
        },
      })
      .willRespondWith({
        status: 400,
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptContent('script-id', ''), (error) => {
        expect(error.status).toStrictEqual(400);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('script id for project that does not exist', () => {
    provider
      .given('script id for project that does not exist for get script content')
      .uponReceiving('a request to get script content')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id/content',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 404,
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptContent('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('project id for project that does not exist', () => {
    provider
      .given('project id for project that does not exist for get script content')
      .uponReceiving('a request to get script content')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id/content',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 404,
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptContent('script-id', 'project-id'), (error) => {
        expect(error.status).toStrictEqual(404);
        expect(error.error.message).toStrictEqual('message');
      })
    );
  });

  it('error while getting script content', () => {
    provider
      .given('error while getting script content')
      .uponReceiving('a request to get script content')
      .withRequest({
        method: 'GET',
        path: '/api/scripts/script-id/content',
        query: {
          projectId: like('project-id'),
        },
      })
      .willRespondWith({
        status: 500,
        body: createErrorResponse(),
      });

    return provider.executeTest(() =>
      executeFailedInteraction(scriptService.getScriptContent('script-id', 'project-id'), (error) => {
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
