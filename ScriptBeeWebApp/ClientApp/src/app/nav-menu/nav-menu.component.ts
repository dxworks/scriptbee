import {Component, OnInit} from '@angular/core';
import {ThemeService} from '../services/theme/theme.service';
import {OverlayContainer} from '@angular/cdk/overlay';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {

  constructor(public themeService: ThemeService, private overlayContainer: OverlayContainer) {
  }

  ngOnInit() {
    this.themeService.darkThemeSubject.subscribe(darkMode => {
      const darkClassName = 'dark-theme-mode';
      if (darkMode) {
        this.overlayContainer.getContainerElement().classList.add(darkClassName);
      } else {
        this.overlayContainer.getContainerElement().classList.remove(darkClassName);
      }
    });
  }
}
