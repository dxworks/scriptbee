import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ThemeService } from '../../services/theme/theme.service';
import { InstanceManagerComponent } from '../instance-manager/instance-manager.component';
import { ProjectStateService } from '../../services/projects/project-state.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  imports: [MatToolbarModule, MatButtonModule, MatIconModule, MatSlideToggleModule, RouterLink, InstanceManagerComponent],
})
export class NavMenuComponent {
  themeService = inject(ThemeService);
  projectState = inject(ProjectStateService);
}
