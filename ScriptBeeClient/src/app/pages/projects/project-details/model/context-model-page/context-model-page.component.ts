import { Component, computed, inject, input } from '@angular/core';
import { CurrentlyLoadedModelsComponent } from './currently-loaded-models/currently-loaded-models.component';
import { MatDivider } from '@angular/material/list';
import { ProjectContextComponent } from './project-context/project-context.component';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';
import { Project } from '../../../../../types/project';

@Component({
  selector: 'app-context-model-page',
  imports: [CurrentlyLoadedModelsComponent, MatDivider, ProjectContextComponent],
  templateUrl: './context-model-page.component.html',
  styleUrl: './context-model-page.component.scss',
})
export class ContextModelPage {
  private projectStateService = inject(ProjectStateService);

  project = input.required<Project>();

  instanceId = computed(() => this.projectStateService.currentInstanceId());
}
