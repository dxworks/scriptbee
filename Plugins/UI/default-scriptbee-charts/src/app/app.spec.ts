import { computed, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { ThemeService } from './services/theme.service';

describe('AppComponent with Mock Theme', () => {
  let mockThemeService: Partial<ThemeService>;

  beforeEach(async () => {
    mockThemeService = {
      isDark: signal(false),
      echartsTheme: computed(() => 'light'),
    };

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{ provide: ThemeService, useValue: mockThemeService }],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });
});
