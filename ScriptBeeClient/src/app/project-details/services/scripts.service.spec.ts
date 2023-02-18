import { ScriptsService } from './scripts.service';
import { createHttpFactory, HttpMethod, SpectatorHttp } from '@ngneat/spectator/jest';

describe('Scripts Service Test', () => {
  let spectator: SpectatorHttp<ScriptsService>;
  const scriptsServiceFactory = createHttpFactory({ service: ScriptsService });

  beforeEach(() => (spectator = scriptsServiceFactory()));

  it('script languages exist', () => {
    spectator.service.getAvailableLanguages().subscribe();

    spectator.expectOne('/api/scripts/languages', HttpMethod.GET);
  });
});
