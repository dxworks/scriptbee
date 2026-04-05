import { Component, inject, input, signal } from '@angular/core';
import { Project } from '../../../../../types/project';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { DeleteProjectDialogComponent } from './delete-project-dialog/delete-project-dialog.component';
import { ProjectService } from '../../../../../services/projects/project.service';
import { Router } from '@angular/router';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-project-danger-zone',
  imports: [MatCardModule, MatButtonModule, LoadingProgressBarComponent],
  templateUrl: './project-danger-zone.component.html',
  styleUrl: './project-danger-zone.component.scss',
})
export class ProjectDangerZoneComponent {
  project = input.required<Project>();
  readonly dialog = inject(MatDialog);

  isDeleteLoading = signal(false);

  private projectService = inject(ProjectService);
  private router = inject(Router);

  openDialog(): void {
    const dialogRef = this.dialog.open(DeleteProjectDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.isDeleteLoading.set(true);
        this.projectService
          .deleteProject(this.project().id)
          .pipe(finalize(() => this.isDeleteLoading.set(false)))
          .subscribe({ next: () => this.router.navigate(['/projects']) });
      }
    });
  }
}
