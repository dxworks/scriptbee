import { beforeEach, describe, expect, it } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';

describe('ThemeServiceService', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      providers: [ThemeService],
    })
  );

  it('should be created', () => {
    const service: ThemeService = TestBed.inject(ThemeService);

    expect(service).toBeTruthy();
  });
});
