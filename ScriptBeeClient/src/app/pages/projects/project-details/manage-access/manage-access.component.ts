import { Component, computed, inject } from '@angular/core';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { MatDividerModule } from '@angular/material/divider';
import { ProjectMembersComponent } from './project-members/project-members.component';
import { ProjectTokensComponent } from './project-tokens/project-tokens.component';

@Component({
  selector: 'app-manage-access',
  templateUrl: './manage-access.component.html',
  styleUrl: './manage-access.component.scss',
  imports: [MatDividerModule, ProjectMembersComponent, ProjectTokensComponent],
})
export class ManageAccessComponent {
  private projectStateService = inject(ProjectStateService);

  projectId = computed(() => this.projectStateService.currentProject()!.id);
}
