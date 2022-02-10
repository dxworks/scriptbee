import { Component } from '@angular/core';
import {ThemeService} from "./services/theme/theme.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  constructor(public themeService: ThemeService) {
  }
}
