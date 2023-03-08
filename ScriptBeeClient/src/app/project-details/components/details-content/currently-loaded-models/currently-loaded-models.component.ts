import { Component } from '@angular/core';
import { LoadersStore } from '../../../stores/loaders-store.service';
import { ProjectStore } from '../../../stores/project-store.service';

@Component({
  selector: 'app-currently-loaded-models',
  templateUrl: './currently-loaded-models.component.html',
  styleUrls: ['./currently-loaded-models.component.scss'],
})
export class CurrentlyLoadedModelsComponent {
  loadedFiles$ = this.loadersStore.loadedFiles;
  projectData$ = this.projectStore.projectData;

  constructor(private loadersStore: LoadersStore, private projectStore: ProjectStore) {}
}
