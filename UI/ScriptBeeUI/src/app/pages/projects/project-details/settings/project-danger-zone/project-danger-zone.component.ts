import { Component, inject, input, signal } from '@angular/core';
import { Project } from '../../../../../types/project';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { DeleteProjectDialogComponent } from './delete-project-dialog/delete-project-dialog.component';
import { ProjectService } from '../../../../../services/projects/project.service';
import { HttpErrorResponse } from '@angular/common/http';
import { DEFAULT_ERROR_RESPONSE } from '../../../../../utils/api';
import { ErrorResponse } from '../../../../../types/api';
import { Router } from '@angular/router';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';

@Component({
  selector: 'app-project-danger-zone',
  imports: [MatCardModule, MatButtonModule, ErrorStateComponent, LoadingProgressBarComponent],
  templateUrl: './project-danger-zone.component.html',
  styleUrl: './project-danger-zone.component.scss',
})
export class ProjectDangerZoneComponent {
  project = input.required<Project>();
  readonly dialog = inject(MatDialog);

  isDeleting = signal(false);
  deletingError = signal<ErrorResponse | undefined>(undefined);

  constructor(
    private projectService: ProjectService,
    private router: Router
  ) {}

  openDialog(): void {
    const dialogRef = this.dialog.open(DeleteProjectDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.deleteProject();
      }
    });
  }

  private deleteProject() {
    this.isDeleting.set(true);
    this.deletingError.set(undefined);

    this.projectService.deleteProject(this.project().id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.router.navigate([`/projects`]).then();
      },
      error: (error: HttpErrorResponse) => {
        this.deletingError.set(error.error ?? DEFAULT_ERROR_RESPONSE);
        this.isDeleting.set(false);
      },
    });
  }
}
