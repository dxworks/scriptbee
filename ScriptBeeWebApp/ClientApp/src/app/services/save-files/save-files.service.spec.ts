import { TestBed } from '@angular/core/testing';

import { SaveFilesService } from './save-files.service';

describe('SaveFilesService', () => {
  let service: SaveFilesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SaveFilesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
