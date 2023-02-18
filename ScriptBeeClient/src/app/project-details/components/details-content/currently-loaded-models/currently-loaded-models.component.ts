import { Component, Input } from '@angular/core';
import { Project } from '../../../../state/project-details/project';

@Component({
  selector: 'app-currently-loaded-models',
  templateUrl: './currently-loaded-models.component.html',
  styleUrls: ['./currently-loaded-models.component.scss'],
})
export class CurrentlyLoadedModelsComponent {
  @Input()
  project: Project | undefined;
}
