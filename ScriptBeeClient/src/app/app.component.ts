import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavMenuComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private matIconRegistry = inject(MatIconRegistry);
  private domSanitizer = inject(DomSanitizer);

  constructor() {
    this.matIconRegistry.addSvgIconSet(this.domSanitizer.bypassSecurityTrustResourceUrl('./assets/mdi.svg'));
  }
}
