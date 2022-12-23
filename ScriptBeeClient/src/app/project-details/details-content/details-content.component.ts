import { Component, OnInit } from '@angular/core';
import { Project } from '../../state/project-details/project';
import {
  selectProjectDetails,
  selectProjectDetailsLoading
} from '../../state/project-details/project-details.selectors';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-details-content',
  templateUrl: './details-content.component.html',
  styleUrls: ['./details-content.component.scss']
})
export class DetailsContentComponent implements OnInit {
  loading = false;
  loadingError = '';
  project: Project | undefined;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.select(selectProjectDetailsLoading).subscribe({
      next: ({ loading, error }) => {
        this.loading = loading ?? false;
        this.loadingError = error ?? '';
      }
    });

    this.store.select(selectProjectDetails).subscribe({
      next: (projectDetails) => {
        this.project = projectDetails;
      }
    });
  }
}
