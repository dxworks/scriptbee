import {Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  private _isDarkTheme = false;

  public darkThemeSubject = new BehaviorSubject(false);

  constructor() {
    this._isDarkTheme = localStorage.getItem('theme') === 'Dark';
    this.darkThemeSubject.next(this._isDarkTheme);
  }

  get isDarkTheme() {
    return this._isDarkTheme;
  }

  set isDarkTheme(theme) {
    this.darkThemeSubject.next(theme);
    this._isDarkTheme = theme;
    localStorage.setItem('theme', this._isDarkTheme ? 'Dark' : 'Light');
  }
}
