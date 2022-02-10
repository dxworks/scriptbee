import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  constructor() {
    this._isDarkTheme = localStorage.getItem('theme') === "Dark";
  }

  private _isDarkTheme: boolean = false;

  get isDarkTheme() {
    return this._isDarkTheme;
  }

  set isDarkTheme(theme) {
    this._isDarkTheme = theme;
    localStorage.setItem('theme',this._isDarkTheme?"Dark":"Light");
  }
}
