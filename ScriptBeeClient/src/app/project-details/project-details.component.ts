import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Store } from "@ngrx/store";
import { selectLoaders, selectLoadersFetchError } from "../state/loaders/loaders.selectors";
import { fetchLoaders } from '../state/loaders/loaders.actions';
import { selectLinkers, selectLinkersFetchError } from "../state/linkers/linkers.selectors";
import { fetchLinkers } from "../state/linkers/linkers.actions";
import { selectProjectDetails } from "../state/project-details/project-details.selectors";
import { fetchProject } from "../state/project-details/project-details.actions";

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.scss']
})
export class ProjectDetailsComponent implements OnInit {

  constructor(private route: ActivatedRoute, private snackBar: MatSnackBar,
              private store: Store) {
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    this.store.select(selectProjectDetails).subscribe(projectDetails => {
      if (!projectDetails) {
        this.store.dispatch(fetchProject({projectId: id}));
      }
    });

    this.store.select(selectLoaders).subscribe(loaders => {
      if (!loaders) {
        this.store.dispatch(fetchLoaders({projectId: id}));
      }
    });

    this.store.select(selectLinkers).subscribe(linkers => {
      if (!linkers) {
        this.store.dispatch(fetchLinkers({projectId: id}));
      }
    });

    this.store.select(selectLoadersFetchError).subscribe(error => {
      if (error) {
        this.snackBar.open('Could not get loaders!', 'Ok', {
          duration: 4000
        });
      }
    });

    this.store.select(selectLinkersFetchError).subscribe(error => {
      if (error) {
        this.snackBar.open('Could not get linkers!', 'Ok', {
          duration: 4000
        });
      }
    });
  }
}
