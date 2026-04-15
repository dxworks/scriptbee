import { DOCUMENT, effect, inject, Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private darkModeSignal = signal(localStorage.getItem('theme') === 'Dark');
  readonly darkMode = this.darkModeSignal.asReadonly();

  constructor() {
    effect(() => {
      localStorage.setItem('theme', this.darkModeSignal() ? 'Dark' : 'Light');
      this.document.documentElement.classList.toggle('darkMode', this.darkModeSignal());
    });
  }

  toggleDarkMode() {
    this.darkModeSignal.update((isDarkMode) => !isDarkMode);
  }
}
