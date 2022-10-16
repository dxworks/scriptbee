import { Component } from '@angular/core';
import { ThemeService } from '../services/theme/theme.service';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {

  constructor(public themeService: ThemeService) {
  }
}
