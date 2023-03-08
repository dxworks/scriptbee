import { Component, OnInit } from '@angular/core';
import { ProjectStore } from '../../stores/project-store.service';
import { UploadModelsStore } from '../../stores/upload-models-store.service';
import { LoadersStore } from '../../stores/loaders-store.service';
import { LinkersStore } from '../../stores/linkers-store.service';
import { ContextStore } from '../../stores/context-store.service';

@Component({
  selector: 'app-details-content',
  templateUrl: './details-content.component.html',
  styleUrls: ['./details-content.component.scss'],
  providers: [UploadModelsStore, LoadersStore, LinkersStore, ContextStore],
})
export class DetailsContentComponent implements OnInit {
  projectData$ = this.projectStore.projectData;
  projectDataLoading$ = this.projectStore.projectDataLoading;
  projectDataError$ = this.projectStore.projectDataError;

  constructor(private projectStore: ProjectStore, private loaderStore: LoadersStore) {}

  ngOnInit(): void {
    this.projectStore.loadProjectData();

    this.projectStore.projectData.subscribe((projectData) => {
      if (projectData) {
        this.loaderStore.setSavedFiles(projectData.savedFiles);
        this.loaderStore.setLoadedFiles(projectData.loadedFiles);
      }
    });
  }
}
