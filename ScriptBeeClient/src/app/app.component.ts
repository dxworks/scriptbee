import { Component, computed, effect, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatIcon, MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { MatSidenavModule } from '@angular/material/sidenav';
import { InstanceManagerComponent } from './components/instance-manager/instance-manager.component';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { MatToolbar } from '@angular/material/toolbar';
import { ProjectStateService } from './services/projects/project-state.service';
import { ProjectSideNavListComponent } from './components/navigation/project-side-nav-list/project-side-nav-list.component';
import { ThemeService } from './services/theme/theme.service';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    MatSidenavModule,
    InstanceManagerComponent,
    MatButton,
    MatIcon,
    MatIconButton,
    MatSlideToggle,
    MatToolbar,
    RouterLink,
    ProjectSideNavListComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private matIconRegistry = inject(MatIconRegistry);
  private domSanitizer = inject(DomSanitizer);
  themeService = inject(ThemeService);
  projectState = inject(ProjectStateService);

  isMenuOpen = signal<boolean>(localStorage.getItem('isMenuOpen') === 'true');

  shouldDisplayMenuHamburger = computed(() => {
    return !!this.projectState.currentProjectId();
  });

  sideNavWidth = computed(() => {
    if (!this.shouldDisplayMenuHamburger()) {
      return '0px';
    }

    return this.isMenuOpen() ? '200px' : '56px';
  });

  constructor() {
    this.matIconRegistry.addSvgIconSet(this.domSanitizer.bypassSecurityTrustResourceUrl('./assets/mdi.svg'));

    effect(() => {
      localStorage.setItem('isMenuOpen', String(this.isMenuOpen()));
    });
  }

  onMenuButtonClick() {
    this.isMenuOpen.update((value) => !value);
  }
}
