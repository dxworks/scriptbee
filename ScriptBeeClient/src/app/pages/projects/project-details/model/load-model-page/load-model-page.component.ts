import { Component, computed, inject } from '@angular/core';
import { MatDivider } from '@angular/material/list';
import { ProjectStateService } from '../../../../../services/projects/project-state.service';
import { LoadModelsComponent } from './load-models/load-models.component';
import { LinkModelsComponent } from './link-models/link-models.component';

@Component({
  selector: 'app-load-model-page',
  imports: [MatDivider, LoadModelsComponent, LinkModelsComponent],
  templateUrl: './load-model-page.component.html',
  styleUrl: './load-model-page.component.scss',
})
export class LoadModelPage {
  private projectStateService = inject(ProjectStateService);

  project = computed(() => this.projectStateService.currentProject()!);
  instanceId = computed(() => this.projectStateService.currentInstanceId());
}
