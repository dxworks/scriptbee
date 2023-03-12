import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProjectStore } from '../stores/project-store.service';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.scss'],
  providers: [ProjectStore],
})
export class ProjectDetailsComponent implements OnInit {
  constructor(private route: ActivatedRoute, private projectStore: ProjectStore) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.projectStore.setProjectId(id);
  }
}
