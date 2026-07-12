import { computed, DOCUMENT, effect, inject, Injectable, signal } from '@angular/core';

const DARK_THEME_NAME = 'dark';
const LIGHT_THEME_NAME = 'light';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private darkModeSignal = signal(localStorage.getItem('theme') === DARK_THEME_NAME);

  readonly darkMode = this.darkModeSignal.asReadonly();
  readonly theme = computed(() => (this.darkModeSignal() ? DARK_THEME_NAME : LIGHT_THEME_NAME));

  constructor() {
    effect(() => {
      localStorage.setItem('theme', this.theme());
      this.document.documentElement.classList.toggle('darkMode', this.darkModeSignal());
    });
  }

  toggleDarkMode() {
    this.darkModeSignal.update((isDarkMode) => !isDarkMode);
  }
}
