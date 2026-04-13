import { Component, computed, inject } from '@angular/core';
import { CurrentlyLoadedModelsComponent } from './currently-loaded-models/currently-loaded-models.component';
import { MatDivider } from '@angular/material/list';
import { ProjectContextComponent } from './project-context/project-context.component';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';

@Component({
  selector: 'app-context-model-page',
  imports: [CurrentlyLoadedModelsComponent, MatDivider, ProjectContextComponent],
  templateUrl: './context-model-page.component.html',
  styleUrl: './context-model-page.component.scss',
})
export class ContextModelPage {
  private projectStateService = inject(ProjectStateService);

  project = computed(() => this.projectStateService.currentProject()!);
  instanceId = computed(() => this.projectStateService.currentInstanceId());
}
