import { TestBed } from '@angular/core/testing';

import { RunScriptService } from './run-script.service';

describe('RunScriptService', () => {
  let service: RunScriptService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RunScriptService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
