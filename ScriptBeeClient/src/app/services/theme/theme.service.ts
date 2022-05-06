import {Inject, Injectable, Renderer2, RendererFactory2} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {DOCUMENT} from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  private _isDarkTheme = false;
  private renderer: Renderer2;
  public darkThemeSubject = new BehaviorSubject(false);

  constructor(@Inject(DOCUMENT) private document: Document, rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);
    this.isDarkTheme = localStorage.getItem('theme') === 'Dark';
  }

  get isDarkTheme() {
    return this._isDarkTheme;
  }

  set isDarkTheme(theme) {
    this._isDarkTheme = theme;
    this.renderer.setAttribute(this.document.body, 'class', (this._isDarkTheme ? 'dark-theme' : 'light-theme') + ' mat-app-background');
    this.darkThemeSubject.next(theme);
    localStorage.setItem('theme', this._isDarkTheme ? 'Dark' : 'Light');

    setTimeout(() => {
      const splitGutterStyles = document.querySelectorAll('.as-split-gutter');
      splitGutterStyles.forEach((gutter: any) => {
        if (theme) {
          gutter.style.backgroundColor = 'var(--primary-color)';
        } else {
          gutter.style.backgroundColor = 'var(--accent-color)';
        }
      });
    }, 10);
  }
}
