import { executeFailedInteraction, executeSuccessfulInteraction, getPactInterceptor, getResponseHeaders, pactWrapper } from '../../../../test/pactUtils';
import { eachLike, like } from '@pact-foundation/pact/src/v3/matchers';
import { TestBed } from '@angular/core/testing';
import { ScriptsService } from './scripts.service';
import { HttpClientModule } from '@angular/common/http';

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
          body: {
            message: like('message'),
            code: like(500),
          },
        });

      return provider.executeTest(() =>
        executeFailedInteraction(scriptService.getAvailableLanguages(), (error) => {
          expect(error.error.message).toEqual('message');
          expect(error.error.code).toEqual(500);
        })
      );
    });
  });
});
