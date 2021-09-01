import {Component} from '@angular/core';
import {ThemeService} from '../services/theme.service';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  constructor(public themeService: ThemeService) { }
}
