import { Component, inject, input } from '@angular/core';
import { Project } from '../../../../../types/project';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { DeleteProjectDialogComponent } from './delete-project-dialog/delete-project-dialog.component';
import { ProjectService } from '../../../../../services/projects/project.service';
import { Router } from '@angular/router';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { apiHandler } from '../../../../../utils/apiHandler';

@Component({
  selector: 'app-project-danger-zone',
  imports: [MatCardModule, MatButtonModule, ErrorStateComponent, LoadingProgressBarComponent],
  templateUrl: './project-danger-zone.component.html',
  styleUrl: './project-danger-zone.component.scss',
})
export class ProjectDangerZoneComponent {
  project = input.required<Project>();
  readonly dialog = inject(MatDialog);

  deleteProjectHandler = apiHandler(
    () => this.projectService.deleteProject(this.project().id),
    () => this.router.navigate(['/projects'])
  );

  constructor(
    private projectService: ProjectService,
    private router: Router
  ) {}

  openDialog(): void {
    const dialogRef = this.dialog.open(DeleteProjectDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.deleteProjectHandler.execute();
      }
    });
  }
}
