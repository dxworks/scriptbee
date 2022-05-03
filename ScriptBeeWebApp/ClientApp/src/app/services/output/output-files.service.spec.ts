import { TestBed } from '@angular/core/testing';

import { OutputFilesService } from './output-files.service';

describe('OutputFilesService', () => {
  let service: OutputFilesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OutputFilesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
