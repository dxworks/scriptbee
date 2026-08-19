import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MatIcon, MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { MatSidenavModule } from '@angular/material/sidenav';
import { InstanceManagerComponent } from './components/instance-manager/instance-manager.component';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { MatToolbar } from '@angular/material/toolbar';
import { ProjectStateService } from './services/projects/project-state.service';
import { ProjectSideNavListComponent } from './components/navigation/project-side-nav-list/project-side-nav-list.component';
import { ThemeService } from './services/common/theme.service';
import { GatewayPluginsService } from './services/plugin/gateway-plugins.service';
import { AsyncPipe } from '@angular/common';
import { AuthService } from './services/auth/AuthService';

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
    AsyncPipe,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  private matIconRegistry = inject(MatIconRegistry);
  private domSanitizer = inject(DomSanitizer);
  private gatewayPluginsService = inject(GatewayPluginsService);
  private router = inject(Router);

  authService = inject(AuthService);
  themeService = inject(ThemeService);
  projectState = inject(ProjectStateService);

  navBarsElements = this.gatewayPluginsService.topNavigationBarOutlets;

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

  ngOnInit(): void {
    this.authService.checkAuth().subscribe();
  }

  onMenuButtonClick() {
    this.isMenuOpen.update((value) => !value);
  }

  logout() {
    this.authService.logout();
    void this.router.navigate(['/login']);
  }
}
