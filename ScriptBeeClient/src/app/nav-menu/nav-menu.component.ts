import {Component, OnInit} from '@angular/core';
import {ThemeService} from '../services/theme/theme.service';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {

  constructor(public themeService: ThemeService) {
  }

  ngOnInit() {
  }
}
