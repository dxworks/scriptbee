import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { selectProjectDetails } from '../../state/project-details/project-details.selectors';
import { fetchProject } from '../../state/project-details/project-details.actions';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.scss'],
})
export class ProjectDetailsComponent implements OnInit {
  constructor(private route: ActivatedRoute, private store: Store) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    this.store.select(selectProjectDetails).subscribe((projectDetails) => {
      if (!projectDetails) {
        this.store.dispatch(fetchProject({ projectId: id }));
      }
    });
  }
}
